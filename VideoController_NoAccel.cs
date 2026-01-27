using System.Collections;
using UnityEngine;
using UnityEngine.Video;

[DisallowMultipleComponent]
public class VideoController_NoAccel : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Timing")]
    [Min(0f)] public float startDelaySeconds = 0f;

    [Header("Options")]
    public bool playOnEnable = true;

    private Coroutine _routine;

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
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("[VideoController_NoAccel] VideoPlayer is not assigned.");
            return;
        }

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        // 强制正常速度（不可加速）
        videoPlayer.playbackSpeed = 1f;

        if (startDelaySeconds > 0f)
            yield return new WaitForSecondsRealtime(startDelaySeconds);

        // 准备并播放（更稳）
        if (!videoPlayer.isPrepared)
        {
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared) yield return null;
        }

        videoPlayer.Play();
    }
}
