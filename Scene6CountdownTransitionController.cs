using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene6CountdownTransitionController : MonoBehaviour
{
    [Header("Enable")]
    public bool enable = true;

    [Header("Countdown")]
    [Tooltip("Seconds after entering Scene6 before starting fade to black.")]
    [Min(0f)] public float delayBeforeFade = 60f;

    [Header("Fade Overlay (to Black only)")]
    [Tooltip("CanvasGroup on FadeOverlay. Alpha 0 = clear, Alpha 1 = black.")]
    public CanvasGroup overlayGroup;

    [Tooltip("If true, force overlay alpha to 0 on Awake (start clear).")]
    public bool forceClearOnAwake = true;

    [Tooltip("Duration of fade to black (alpha 0 -> 1).")]
    [Min(0f)] public float fadeToBlackDuration = 1.5f;

    [Header("Next Scene")]
    [Tooltip("Scene name to load after fade is complete. Must be added to Build Settings.")]
    public string nextSceneName;

    [Header("Options")]
    [Tooltip("Use unscaled time so it works even if Time.timeScale changes.")]
    public bool useUnscaledTime = true;

    private Coroutine _routine;

    private void Awake()
    {
        if (!enable) return;

        if (!overlayGroup)
        {
            Debug.LogError("Scene6CountdownTransitionController: Missing overlayGroup.");
            return;
        }

        if (forceClearOnAwake)
            overlayGroup.alpha = 0f;
    }

    private void Start()
    {
        if (!enable) return;

        if (!overlayGroup)
            return;

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("Scene6CountdownTransitionController: nextSceneName is empty.");
            return;
        }

        // Optional safety check (avoids silent failure)
        if (!UnityEngine.Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogError($"Scene6CountdownTransitionController: Scene not in Build Settings: {nextSceneName}");
            return;
        }

        _routine = StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        if (delayBeforeFade > 0f)
            yield return WaitSeconds(delayBeforeFade);

        // Fade to black ONLY
        yield return Fade(overlayGroup, overlayGroup.alpha, 1f, fadeToBlackDuration);

        // Load next scene immediately after fully black
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator WaitSeconds(float seconds)
    {
        if (seconds <= 0f) yield break;

        if (useUnscaledTime)
            yield return new WaitForSecondsRealtime(seconds);
        else
            yield return new WaitForSeconds(seconds);
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
