using System.Collections;
using UnityEngine;

public class Scene5OverlayTimedFadeController : MonoBehaviour
{
    [Header("Enable")]
    public bool enable = true;

    [Header("Target")]
    [Tooltip("CanvasGroup on your FadeOverlay (black full-screen Image). Alpha=0 transparent, Alpha=1 black.")]
    public CanvasGroup overlayGroup;

    [Header("Base Timeline")]
    [Tooltip("Global offset from scene start (seconds). All times below will be added on top of this.")]
    [Min(0f)] public float startDelay = 0f;

    [Header("Phase A: Fade TO Black")]
    [Tooltip("Absolute time (seconds) from scene start + startDelay to begin fading to black.")]
    [Min(0f)] public float fadeToBlackAt = 20f;

    [Tooltip("Duration of fading to black (alpha 0 -> 1).")]
    [Min(0f)] public float fadeToBlackDuration = 1.0f;

    [Header("Phase B: Fade BACK to Clear")]
    [Tooltip("Absolute time (seconds) from scene start + startDelay to begin fading back to clear.")]
    [Min(0f)] public float fadeToClearAt = 24f;

    [Tooltip("Duration of fading back to clear (alpha 1 -> 0).")]
    [Min(0f)] public float fadeToClearDuration = 1.0f;

    [Header("Options")]
    [Tooltip("Force overlay alpha to 0 on Awake (starts clear).")]
    public bool forceClearOnAwake = true;

    [Tooltip("Use unscaled time so timing is stable during cutscenes (ignores Time.timeScale).")]
    public bool useUnscaledTime = true;

    private float _sceneStartTime;
    private Coroutine _routine;

    private void Awake()
    {
        _sceneStartTime = Now();

        if (!enable) return;

        if (!overlayGroup)
        {
            Debug.LogError("Scene5OverlayTimedFadeController: Missing overlayGroup.");
            return;
        }

        if (forceClearOnAwake)
            overlayGroup.alpha = 0f;
    }

    private void OnEnable()
    {
        if (!enable) return;
        if (!overlayGroup) return;

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(RunTimeline());
    }

    private IEnumerator RunTimeline()
    {
        // Safety: ensure timeline order makes sense.
        // If user sets fadeToClearAt earlier than fadeToBlackAt, we still run in that order by time.
        float tFadeToBlack = startDelay + fadeToBlackAt;
        float tFadeToClear = startDelay + fadeToClearAt;

        // Wait until Phase A start
        yield return WaitUntilElapsed(tFadeToBlack);

        // Phase A: clear -> black
        yield return Fade(overlayGroup, overlayGroup.alpha, 1f, fadeToBlackDuration);

        // Wait until Phase B start (absolute time)
        yield return WaitUntilElapsed(tFadeToClear);

        // Phase B: black -> clear
        yield return Fade(overlayGroup, overlayGroup.alpha, 0f, fadeToClearDuration);
    }

    // -------- Timing helpers --------

    private float Now()
    {
        return useUnscaledTime ? Time.unscaledTime : Time.time;
    }

    private float Elapsed()
    {
        return Now() - _sceneStartTime;
    }

    private IEnumerator WaitUntilElapsed(float targetSeconds)
    {
        if (targetSeconds <= 0f) yield break;

        while (Elapsed() < targetSeconds)
            yield return null;
    }

    private IEnumerator Fade(CanvasGroup g, float from, float to, float duration)
    {
        if (!g) yield break;

        g.alpha = from;

        if (duration <= 0f)
        {
            g.alpha = to;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            g.alpha = Mathf.Lerp(from, to, p);
            yield return null;
        }

        g.alpha = to;
    }
}
