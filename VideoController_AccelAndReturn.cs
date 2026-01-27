using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[DisallowMultipleComponent]
public class VideoController_AccelAndReturn : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Timing")]
    [Min(0f)] public float startDelaySeconds = 0f;
    [Min(0f)] public float returnDelaySeconds = 1f;

    [Header("Fast Forward (Hold Key)")]
    public bool enableFastForward = true;
    public KeyCode fastForwardKey = KeyCode.G;
    [Min(1f)] public float normalSpeed = 1f;
    [Min(1f)] public float fastSpeedMultiplier = 4f; // 可调：按住G => 4倍

    [Header("Return To Menu")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Options")]
    public bool playOnEnable = true;

    private Coroutine _playRoutine;
    private bool _isWatchingEnd = false;
    private bool _returnTriggered = false;

    private void Reset()
    {
        videoPlayer = GetComponentInChildren<VideoPlayer>();
    }

    private void OnEnable()
    {
        if (playOnEnable)
            Play();
    }

    private void OnDisable()
    {
        if (_playRoutine != null)
        {
            StopCoroutine(_playRoutine);
            _playRoutine = null;
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }

        _isWatchingEnd = false;
        _returnTriggered = false;
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("[VideoController_AccelAndReturn] VideoPlayer is not assigned.");
            return;
        }

        // 确保不会重复订阅
        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.loopPointReached += OnVideoFinished;

        _returnTriggered = false;

        if (_playRoutine != null) StopCoroutine(_playRoutine);
        _playRoutine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        // 开始前统一设为正常速度
        videoPlayer.playbackSpeed = normalSpeed;

        if (startDelaySeconds > 0f)
            yield return new WaitForSecondsRealtime(startDelaySeconds);

        // Prepare 更稳
        if (!videoPlayer.isPrepared)
        {
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared) yield return null;
        }

        videoPlayer.Play();
        _isWatchingEnd = true;

        // 播放期间持续检查按键加速
        while (_isWatchingEnd && videoPlayer != null && videoPlayer.isPlaying)
        {
            ApplyFastForwardIfNeeded();
            yield return null;
        }

        // 如果视频结束事件没触发（极少数情况），这里再补一刀
        if (!_returnTriggered && videoPlayer != null && !videoPlayer.isPlaying)
        {
            // 可能已经到末尾
            StartCoroutine(ReturnToMenuAfterDelay());
        }
    }

    private void Update()
    {
        // 保险：如果播放期间 Update 能跑，也给一次键控（避免某些平台 loopPointReached 之前 isPlaying 状态变化）
        if (_isWatchingEnd && videoPlayer != null && videoPlayer.isPlaying)
        {
            ApplyFastForwardIfNeeded();
        }
    }

    private void ApplyFastForwardIfNeeded()
    {
        if (!enableFastForward || videoPlayer == null) return;

        bool holding = Input.GetKey(fastForwardKey);
        float targetSpeed = holding ? (normalSpeed * fastSpeedMultiplier) : normalSpeed;

        // VideoPlayer 支持的速度通常是 >0；某些平台可能对超高倍数有限制
        videoPlayer.playbackSpeed = Mathf.Max(0.1f, targetSpeed);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (_returnTriggered) return;
        _returnTriggered = true;
        _isWatchingEnd = false;

        // 结束时复位速度，避免影响下次
        if (videoPlayer != null)
            videoPlayer.playbackSpeed = normalSpeed;

        StartCoroutine(ReturnToMenuAfterDelay());
    }

    private IEnumerator ReturnToMenuAfterDelay()
    {
        if (returnDelaySeconds > 0f)
            yield return new WaitForSecondsRealtime(returnDelaySeconds);

        // 直接切回 MainMenu
        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            Debug.LogError("[VideoController_AccelAndReturn] mainMenuSceneName is empty.");
            yield break;
        }

        SceneManager.LoadScene(mainMenuSceneName.Trim());
    }
}
