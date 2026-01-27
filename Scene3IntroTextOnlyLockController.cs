using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scene3IntroTextOnlyLockController : MonoBehaviour
{
    [System.Serializable]
    public class TextStep
    {
        [Header("Target UI")]
        public TMP_Text text;             // Drag a TMP_Text
        public CanvasGroup textGroup;     // Drag the CanvasGroup controlling this text alpha

        [Header("Content")]
        [TextArea(1, 8)]
        public string content = "Text";

        [Header("Timeline (Absolute from Scene Start)")]
        [Min(0f)] public float sceneDelay = 0f;  // Absolute seconds from scene start

        [Header("Timings (per step)")]
        [Min(0f)] public float fadeIn = 0.6f;
        [Min(0f)] public float hold = 1.0f;
        [Min(0f)] public float fadeOut = 0.6f;

        public float EndTime => sceneDelay + fadeIn + hold + fadeOut;
    }

    public enum UnlockMode
    {
        AfterAllTextsFinished,   // Unlock when last text finishes
        AtExactTime              // Unlock at a specified absolute time
    }

    [Header("Text Steps (Absolute Timeline, Can Overlap)")]
    public List<TextStep> steps = new List<TextStep>();

    [Header("Player Lock (Disable during intro)")]
    [Tooltip("Drag PlayerMove, MouseLook, etc. Disabled at start and re-enabled when unlocked.")]
    public Behaviour[] disableBehavioursOnStart;

    [Header("Unlock Control")]
    public UnlockMode unlockMode = UnlockMode.AfterAllTextsFinished;

    [Tooltip("Only used when UnlockMode = AtExactTime. Absolute seconds from scene start.")]
    [Min(0f)] public float unlockAtTime = 0f;

    [Header("Options")]
    public bool playOnStart = true;
    public bool useUnscaledTime = true;

    private float _sceneStartTime;

    private void Awake()
    {
        _sceneStartTime = Now();

        // Hide all text at start and prefill content to prevent flicker
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

        // Lock immediately
        SetBehavioursEnabled(false);
    }

    private void Start()
    {
        if (playOnStart)
            StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        // Start all steps concurrently
        float latestEnd = 0f;

        if (steps != null && steps.Count > 0)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                var s = steps[i];
                if (s == null) continue;

                if (!s.text || !s.textGroup)
                {
                    Debug.LogError($"Scene3IntroTextOnlyLockController: Step {i} missing TMP_Text or CanvasGroup.");
                    continue;
                }

                if (s.EndTime > latestEnd) latestEnd = s.EndTime;

                StartCoroutine(PlayStep(s));
            }
        }

        // Unlock timing
        float unlockTime = (unlockMode == UnlockMode.AfterAllTextsFinished) ? latestEnd : unlockAtTime;

        if (unlockTime > 0f)
            yield return WaitUntilElapsed(unlockTime);

        // Unlock
        SetBehavioursEnabled(true);
    }

    private IEnumerator PlayStep(TextStep s)
    {
        // Wait until step's absolute time
        yield return WaitUntilElapsed(s.sceneDelay);

        // Set text
        s.text.text = s.content ?? "";

        // Fade in
        yield return Fade(s.textGroup, 0f, 1f, s.fadeIn);

        // Hold
        if (s.hold > 0f)
            yield return WaitSeconds(s.hold);

        // Fade out
        yield return Fade(s.textGroup, 1f, 0f, s.fadeOut);
    }

    // -------- helpers --------

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
