using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneAutoTransitionMultiText_V3 : MonoBehaviour
{
    [System.Serializable]
    public class TextStep
    {
        [Header("Target UI")]
        public TMP_Text text;             // Drag a TMP_Text (each step can be different)
        public CanvasGroup textGroup;     // Drag the CanvasGroup for this text (on the text or its parent)

        [Header("Content")]
        [TextArea(1, 6)]
        public string content = "Text";

        [Header("Timeline (Absolute from Scene Start)")]
        [Min(0f)] public float sceneDelay = 0f;  // Absolute time (seconds) from scene start

        [Header("Timings (per step)")]
        [Min(0f)] public float fadeIn = 0.6f;
        [Min(0f)] public float hold = 1.0f;
        [Min(0f)] public float fadeOut = 0.6f;
    }

    [Header("Overlay Fade (Black Screen)")]
    public CanvasGroup overlayGroup;             // Black overlay (FadeOverlay)
    [Min(0f)] public float enterHoldBlack = 0f;  // Hold black at start (from scene start)
    [Min(0f)] public float overlayFadeIn = 1f;   // Black -> transparent
    [Min(0f)] public float overlayFadeOut = 1f;  // Transparent -> black
    [Min(0f)] public float exitHoldBlack = 0f;   // Hold black at end

    [Header("Steps (Unlimited, Can Overlap)")]
    public List<TextStep> steps = new List<TextStep>();

    [Header("Next Scene")]
    public string nextSceneName;

    [Header("Options")]
    public bool playOnStart = true;
    public bool useUnscaledTime = true;          // Recommended for cutscenes (ignores Time.timeScale)

    private float _sceneStartTime;

    private void Awake()
    {
        // Record scene start time as early as possible
        _sceneStartTime = Now();

        // Start fully black
        if (overlayGroup) overlayGroup.alpha = 1f;

        // Hide all text groups initially and prefill text to avoid flicker
        if (steps != null)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                var s = steps[i];
                if (s == null) continue;

                if (s.textGroup) s.textGroup.alpha = 0f;
                if (s.text) s.text.text = s.content ?? "";
            }
        }
    }

    private void Start()
    {
        // In case Start happens later than Awake, we still use the same base time
        // so "sceneDelay" is counted from scene entry consistently.

        if (playOnStart)
            StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        if (!overlayGroup)
        {
            Debug.LogError("CutsceneAutoTransitionMultiText_V3: Missing overlayGroup.");
            yield break;
        }

        // 1) Overlay: hold black then fade in (relative to timeline as well)
        // We do this sequentially because it's a single overlay.
        if (enterHoldBlack > 0f)
            yield return WaitSeconds(enterHoldBlack);

        yield return Fade(overlayGroup, 1f, 0f, overlayFadeIn);

        // 2) Start all steps concurrently (absolute timeline)
        float latestStepEnd = 0f;

        if (steps != null && steps.Count > 0)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                var s = steps[i];
                if (s == null) continue;

                // compute expected end time for sequencing exit
                float stepEnd = s.sceneDelay + s.fadeIn + s.hold + s.fadeOut;
                if (stepEnd > latestStepEnd) latestStepEnd = stepEnd;

                StartCoroutine(PlayStep(i, s));
            }
        }

        // 3) Wait until the last step should be finished (absolute)
        // Note: If you want overlay fade-out to start earlier, reduce latestStepEnd by design.
        if (latestStepEnd > 0f)
            yield return WaitUntilElapsed(latestStepEnd);

        // 4) Fade overlay out (transparent -> black), then optional hold
        yield return Fade(overlayGroup, 0f, 1f, overlayFadeOut);

        if (exitHoldBlack > 0f)
            yield return WaitSeconds(exitHoldBlack);

        // 5) Load next scene
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("CutsceneAutoTransitionMultiText_V3: nextSceneName is empty.");
            yield break;
        }

        if (!UnityEngine.Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogError($"CutsceneAutoTransitionMultiText_V3: Scene not in Build Settings: {nextSceneName}");
            yield break;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator PlayStep(int index, TextStep s)
    {
        // Validate references
        if (s.text == null || s.textGroup == null)
        {
            Debug.LogError($"CutsceneAutoTransitionMultiText_V3: Step {index} missing TMP_Text or CanvasGroup.");
            yield break;
        }

        // Wait until absolute time reaches sceneDelay
        yield return WaitUntilElapsed(s.sceneDelay);

        // Set content (font/size/position controlled on TMP component)
        s.text.text = s.content ?? "";

        // Fade in
        yield return Fade(s.textGroup, 0f, 1f, s.fadeIn);

        // Hold
        if (s.hold > 0f)
            yield return WaitSeconds(s.hold);

        // Fade out
        yield return Fade(s.textGroup, 1f, 0f, s.fadeOut);
    }

    // ---------- Timing Helpers (absolute timeline) ----------

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
