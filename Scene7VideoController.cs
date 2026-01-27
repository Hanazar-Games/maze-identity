using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[DisallowMultipleComponent]
public class Scene7VideoSequenceController : MonoBehaviour
{
    [Header("Video Players")]
    public VideoPlayer video1;   // Intro (no fast)
    public VideoPlayer video2;   // Reflection (fast-forward allowed)

    [Header("Sequence Timing")]
    [Min(0f)] public float startDelayBeforeVideo1 = 0f;
    [Min(0f)] public float intervalTimeDelay = 1f;
    [Min(0f)] public float returnDelayAfterVideo2 = 1f;

    [Header("Fast Forward (Video2 Only)")]
    public bool enableFastForwardVideo2 = true;
    public KeyCode fastForwardKey = KeyCode.G;
    [Min(0.1f)] public float normalSpeed = 1f;
    [Min(1f)] public float fastSpeedMultiplier = 4f;

    [Header("Return Scene")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Time Mode")]
    public bool useUnscaledTime = true;

    [Header("Auto Play")]
    public bool playOnStart = true;

    [Header("Reliability")]
    [Min(0.0f)] public double endTimeEpsilonSeconds = 0.05;

    private Coroutine _co;
    private bool _video1Ended;
    private bool _video2Ended;

    private void Reset()
    {
        var vps = GetComponentsInChildren<VideoPlayer>(true);
        if (vps.Length > 0) video1 = vps[0];
        if (vps.Length > 1) video2 = vps[1];
    }

    private void Start()
    {
        if (playOnStart) PlaySequence();
    }

    private void OnDisable() => StopAll();
    private void OnDestroy() => StopAll();

    private void StopAll()
    {
        if (_co != null)
        {
            StopCoroutine(_co);
            _co = null;
        }
        UnsubscribeEvents();
        _video1Ended = false;
        _video2Ended = false;
    }

    [ContextMenu("Play Sequence")]
    public void PlaySequence()
    {
        if (video1 == null || video2 == null)
        {
            Debug.LogError("[Scene7VideoSequenceController] video1/video2 not assigned.");
            return;
        }

        StopAll();

        if (!video1.gameObject.activeInHierarchy) video1.gameObject.SetActive(true);
        if (!video2.gameObject.activeInHierarchy) video2.gameObject.SetActive(true);

        // 关键：先把两个都停掉并清干净
        SafeStop(video1);
        SafeStop(video2);

        // 关键：开始时确保 video1 可见、video2 不抢镜头
        ForceCameraOverlay(video1, 1f);
        ForceCameraOverlay(video2, 0f);

        SubscribeEvents();

        _co = StartCoroutine(SequenceRoutine());
    }

    private void SubscribeEvents()
    {
        UnsubscribeEvents();
        video1.loopPointReached += OnVideo1Finished;
        video2.loopPointReached += OnVideo2Finished;

        // 额外：把错误吐出来（非常关键）
        video1.errorReceived += OnVideoError;
        video2.errorReceived += OnVideoError;
    }

    private void UnsubscribeEvents()
    {
        if (video1 != null)
        {
            video1.loopPointReached -= OnVideo1Finished;
            video1.errorReceived -= OnVideoError;
        }
        if (video2 != null)
        {
            video2.loopPointReached -= OnVideo2Finished;
            video2.errorReceived -= OnVideoError;
        }
    }

    private void OnVideoError(VideoPlayer vp, string msg)
    {
        Debug.LogError($"[Scene7VideoSequenceController] VideoPlayer ERROR on '{vp.gameObject.name}': {msg}");
    }

    private void OnVideo1Finished(VideoPlayer vp) => _video1Ended = true;
    private void OnVideo2Finished(VideoPlayer vp) => _video2Ended = true;

    private IEnumerator SequenceRoutine()
    {
        if (startDelayBeforeVideo1 > 0f)
            yield return Wait(startDelayBeforeVideo1);

        // -------- Video1 --------
        _video1Ended = false;

        yield return PrepareIfNeeded(video1);

        // 确保不循环、速度正常、从头播
        video1.isLooping = false;
        video1.playbackSpeed = 1f;
        video1.time = 0;

        // 可见
        ForceCameraOverlay(video1, 1f);
        // 避免 video2 抢 NearPlane
        ForceCameraOverlay(video2, 0f);

        video1.Play();

        yield return WaitUntilVideoFinishedReliable(video1, () => _video1Ended);

        // 关键修复：Video1 播完后，把它彻底“交出” NearPlane
        ForceCameraOverlay(video1, 0f);
        SafeStop(video1);
        video1.enabled = false; // 关键：禁用组件，避免最后一帧残留占用 NearPlane

        if (intervalTimeDelay > 0f)
            yield return Wait(intervalTimeDelay);

        // -------- Video2 --------
        _video2Ended = false;

        // 重新启用 video2（以防你场景里关过）
        video2.enabled = true;

        yield return PrepareIfNeeded(video2);

        video2.isLooping = false;
        video2.playbackSpeed = normalSpeed;
        video2.time = 0;

        // 关键：Video2 开播前强制可见
        ForceCameraOverlay(video2, 1f);

        video2.Play();

        while (!_video2Ended)
        {
            if (video2 != null && video2.isPlaying)
                ApplyFastForwardForVideo2();

            if (IsNearEnd(video2))
            {
                _video2Ended = true;
                break;
            }

            yield return null;
        }

        if (video2 != null) video2.playbackSpeed = normalSpeed;

        if (returnDelayAfterVideo2 > 0f)
            yield return Wait(returnDelayAfterVideo2);

        SceneManager.LoadScene(mainMenuSceneName.Trim());
    }

    private void SafeStop(VideoPlayer vp)
    {
        if (vp == null) return;
        try
        {
            vp.Stop();
            vp.time = 0;
        }
        catch { }
    }

    /// <summary>
    /// 如果 RenderMode 是 CameraNearPlane/CameraFarPlane，就用 targetCameraAlpha 强制显示/隐藏。
    /// 其他 RenderMode 不影响。
    /// </summary>
    private void ForceCameraOverlay(VideoPlayer vp, float alpha)
    {
        if (vp == null) return;

        if (vp.renderMode == VideoRenderMode.CameraNearPlane ||
            vp.renderMode == VideoRenderMode.CameraFarPlane)
        {
            // 这个属性在 Inspector 里叫 “Alpha”
            vp.targetCameraAlpha = Mathf.Clamp01(alpha);
        }
    }

    private IEnumerator PrepareIfNeeded(VideoPlayer vp)
    {
        if (vp == null) yield break;

        // 有些时候 isPrepared 永远不变，先 Prepare 再等
        if (!vp.isPrepared)
        {
            vp.Prepare();
            while (!vp.isPrepared)
                yield return null;
        }
    }

    private IEnumerator WaitUntilVideoFinishedReliable(VideoPlayer vp, System.Func<bool> endedFlag)
    {
        if (vp == null) yield break;

        // 等它真正进入播放（避免刚 Play 就被判定结束）
        float guard = 0f;
        while (!vp.isPlaying && guard < 2f)
        {
            guard += Time.unscaledDeltaTime;
            yield return null;
        }

        while (true)
        {
            if (endedFlag != null && endedFlag()) break;
            if (IsNearEnd(vp)) break;

            // 保底：停止播放且 time 推进过，也算结束
            if (!vp.isPlaying && vp.time > 0.01f) break;

            yield return null;
        }
    }

    private bool IsNearEnd(VideoPlayer vp)
    {
        if (vp == null) return false;

        double len = vp.length;
        if (len > 0.0001 && vp.time >= len - endTimeEpsilonSeconds)
            return true;

        if (vp.frameCount > 0 && vp.frame >= (long)vp.frameCount - 1)
            return true;

        return false;
    }

    private void ApplyFastForwardForVideo2()
    {
        if (!enableFastForwardVideo2) return;
        if (video2 == null) return;

        bool holding = Input.GetKey(fastForwardKey);
        float targetSpeed = holding ? (normalSpeed * fastSpeedMultiplier) : normalSpeed;
        video2.playbackSpeed = Mathf.Max(0.1f, targetSpeed);
    }

    private IEnumerator Wait(float seconds)
    {
        if (seconds <= 0f) yield break;

        if (useUnscaledTime)
            yield return new WaitForSecondsRealtime(seconds);
        else
            yield return new WaitForSeconds(seconds);
    }
}
