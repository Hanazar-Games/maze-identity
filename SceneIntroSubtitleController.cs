using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneIntroSubtitleController : MonoBehaviour
{
    [System.Serializable]
    public class TextStep
    {
        [Header("Target UI")]
        public TMP_Text text;
        public CanvasGroup textGroup;

        [Header("Content")]
        [TextArea(1, 6)]
        public string content = "Text";

        [Header("Timeline (Absolute from Text Timeline Start)")]
        [Min(0f)] public float sceneDelay = 0f;

        [Header("Timings (per step)")]
        [Min(0f)] public float fadeIn = 0.6f;
        [Min(0f)] public float hold = 1.0f;
        [Min(0f)] public float fadeOut = 0.6f;

        public float EndTime => sceneDelay + fadeIn + hold + fadeOut;
    }

    public enum UnlockMode
    {
        AfterAllTextsFinished,
        AtExactTime
    }

    [Header("Overlay (Black Screen)")]
    [Tooltip("CanvasGroup on a full-screen black Image. Alpha=1 means black.")]
    public CanvasGroup overlayGroup;

    [Tooltip("Start overlay alpha at 1 (black) in Awake.")]
    public bool forceOverlayBlackOnAwake = true;

    [Tooltip("Hold fully black at the very beginning (seconds).")]
    [Min(0f)] public float enterHoldBlack = 0f;

    [Tooltip("Fade OUT black overlay first (alpha 1 -> 0).")]
    [Min(0f)] public float overlayFadeOutBlack = 1.0f;

    [Tooltip("If true, the text timeline starts AFTER overlay fade-out completes.")]
    public bool startTextAfterOverlayFadeOut = true;

    [Header("Text Steps (Absolute Timeline, Can Overlap)")]
    public List<TextStep> steps = new List<TextStep>();

    [Header("Player Lock (Optional)")]
    [Tooltip("Drag PlayerMove, MouseLook, etc. Disabled at start and re-enabled at unlock.")]
    public Behaviour[] disableBehavioursOnStart;

    [Header("Unlock Control")]
    public UnlockMode unlockMode = UnlockMode.AfterAllTextsFinished;

    [Tooltip("Only used when UnlockMode = AtExactTime. Seconds from text timeline start.")]
    [Min(0f)] public float unlockAtTime = 0f;

    [Header("Options")]
    public bool playOnStart = true;
    public bool useUnscaledTime = true;

    private float _sceneStartTime;
    private float _textTimelineOffset; // absolute seconds since scene start

    private void Awake()
    {
        _sceneStartTime = Now();

        // Overlay initial state
        if (overlayGroup && forceOverlayBlackOnAwake)
            overlayGroup.alpha = 1f;

        // Hide all text initially + prefill
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

        // Lock immediately (if any)
        SetBehavioursEnabled(false);
    }

    private void Start()
    {
        if (playOnStart)
            StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        if (!overlayGroup)
        {
            Debug.LogError("SceneIntroSubtitleController: Missing overlayGroup.");
            yield break;
        }

        // 1) Hold black
        if (enterHoldBlack > 0f)
            yield return WaitSeconds(enterHoldBlack);

        // 2) Fade out black (black -> visible)
        yield return Fade(overlayGroup, 1f, 0f, overlayFadeOutBlack);

        // 3) Decide when text timeline starts
        _textTimelineOffset = startTextAfterOverlayFadeOut ? Elapsed() : 0f;

        // 4) Start all steps concurrently
        float latestEnd = 0f;

        if (steps != null && steps.Count > 0)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                var s = steps[i];
                if (s == null) continue;

                if (!s.text || !s.textGroup)
                {
                    Debug.LogError($"SceneIntroSubtitleController: Step {i} missing TMP_Text or CanvasGroup.");
                    continue;
                }

                if (s.EndTime > latestEnd) latestEnd = s.EndTime;

                StartCoroutine(PlayStep(s));
            }
        }

        // 5) Unlock time (relative to text timeline start)
        float unlockTime = (unlockMode == UnlockMode.AfterAllTextsFinished) ? latestEnd : unlockAtTime;

        // Convert to absolute scene elapsed time
        float absoluteUnlock = _textTimelineOffset + unlockTime;

        if (absoluteUnlock > 0f)
            yield return WaitUntilElapsed(absoluteUnlock);

        SetBehavioursEnabled(true);
    }

    private IEnumerator PlayStep(TextStep s)
    {
        float absoluteStart = _textTimelineOffset + s.sceneDelay;
        yield return WaitUntilElapsed(absoluteStart);

        s.text.text = s.content ?? "";

        yield return Fade(s.textGroup, 0f, 1f, s.fadeIn);

        if (s.hold > 0f)
            yield return WaitSeconds(s.hold);

        yield return Fade(s.textGroup, 1f, 0f, s.fadeOut);
    }

    // ---------- helpers ----------

    private void SetBehavioursEnabled(bool enabled)
    {
        if (disableBehavioursOnStart == null) return;

        for (int i = 0; i < disableBehavioursOnStart.Length; i++)
        {
            var b = disableBehavioursOnStart[i];
            if (b) b.enabled = enabled;
        }
    }

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
