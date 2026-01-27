using System.Collections;
using TMPro;
using UnityEngine;

public class IntroTwoTextTimelineController : MonoBehaviour
{
    [Header("Enable")]
    public bool enable = true;

    [Header("Options")]
    [Tooltip("Use unscaled time so timing is stable even if Time.timeScale changes.")]
    public bool useUnscaledTime = true;

    [Header("Text 1 (Absolute from Scene Start)")]
    public TMP_Text text1;
    public CanvasGroup group1;
    [TextArea(1, 6)] public string content1 = "Text 1";
    [Min(0f)] public float delay1 = 0f;
    [Min(0f)] public float fadeIn1 = 1f;
    [Min(0f)] public float hold1 = 2f;
    [Min(0f)] public float fadeOut1 = 1f;

    [Header("Text 2 (Absolute from Scene Start)")]
    public TMP_Text text2;
    public CanvasGroup group2;
    [TextArea(1, 6)] public string content2 = "Text 2";
    [Min(0f)] public float delay2 = 0f;
    [Min(0f)] public float fadeIn2 = 1f;
    [Min(0f)] public float hold2 = 2f;
    [Min(0f)] public float fadeOut2 = 1f;

    [Header("Startup Visibility")]
    [Tooltip("Force both text groups hidden on Awake/OnEnable.")]
    public bool forceHiddenOnStart = true;

    private float _sceneStartTime;

    private void Awake()
    {
        _sceneStartTime = Now();

        if (forceHiddenOnStart)
        {
            if (group1) group1.alpha = 0f;
            if (group2) group2.alpha = 0f;
        }

        // Pre-apply content so you can preview in Play Mode quickly
        if (text1) text1.text = content1;
        if (text2) text2.text = content2;
    }

    private void OnEnable()
    {
        if (!enable) return;

        if (forceHiddenOnStart)
        {
            if (group1) group1.alpha = 0f;
            if (group2) group2.alpha = 0f;
        }
    }

    private void Start()
    {
        if (!enable) return;

        if (!ValidateRefs(text1, group1, "Text1")) return;
        if (!ValidateRefs(text2, group2, "Text2")) return;

        StartCoroutine(PlayOne(text1, group1, content1, delay1, fadeIn1, hold1, fadeOut1));
        StartCoroutine(PlayOne(text2, group2, content2, delay2, fadeIn2, hold2, fadeOut2));
    }

    private bool ValidateRefs(TMP_Text t, CanvasGroup g, string label)
    {
        if (!t || !g)
        {
            Debug.LogError($"IntroTwoTextTimelineController: Missing reference(s) for {label}. " +
                           $"Need TMP_Text and CanvasGroup.");
            return false;
        }
        return true;
    }

    private IEnumerator PlayOne(TMP_Text t, CanvasGroup g, string content,
                                float absoluteDelay, float fadeIn, float hold, float fadeOut)
    {
        // Ensure hidden at start
        g.alpha = 0f;

        // Wait until absolute time from scene start
        yield return WaitUntilElapsed(absoluteDelay);

        // Apply content at playback moment
        t.text = content;

        // Fade in
        yield return Fade(g, 0f, 1f, fadeIn);

        // Hold
        if (hold > 0f)
            yield return WaitSeconds(hold);

        // Fade out
        yield return Fade(g, 1f, 0f, fadeOut);

        // Keep hidden
        g.alpha = 0f;
    }

    // ---------- Time helpers ----------
    private float Now() => useUnscaledTime ? Time.unscaledTime : Time.time;

    private float Elapsed() => Now() - _sceneStartTime;

    private IEnumerator WaitUntilElapsed(float target)
    {
        if (target <= 0f) yield break;

        while (Elapsed() < target)
            yield return null;
    }

    private IEnumerator WaitSeconds(float seconds)
    {
        if (seconds <= 0f) yield break;

        if (useUnscaledTime) yield return new WaitForSecondsRealtime(seconds);
        else yield return new WaitForSeconds(seconds);
    }

    private IEnumerator Fade(CanvasGroup g, float from, float to, float duration)
    {
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
