using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scene1IntroLockAndText_V2 : MonoBehaviour
{
    // ✅ Options 返回标记（全局通用）
    public const string KEY_RETURNING_FROM_OPTIONS = "MI_ReturningFromOptions";

    // ✅ “只播一次”标记（按场景/脚本名区分，避免冲突）
    private const string KEY_INTRO_DONE = "MI_Scene1IntroDone";

    [System.Serializable]
    public class TextStep
    {
        [Header("Target UI")]
        public TMP_Text text;             // Drag a TMP_Text
        public CanvasGroup textGroup;     // Drag the CanvasGroup controlling this text alpha

        [Header("Content")]
        [TextArea(1, 6)]
        public string content = "Text";

        [Header("Timeline (Absolute from Intro Start)")]
        [Min(0f)] public float sceneDelay = 0f;  // Absolute seconds from intro start

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

    [Tooltip("Hold fully black at the very beginning (seconds).")]
    [Min(0f)] public float enterHoldBlack = 0f;

    [Tooltip("Fade OUT the black overlay (alpha 1 -> 0). This is the 'black to visible' fade.")]
    [Min(0f)] public float overlayFadeOutBlack = 1.0f;

    [Header("Text Steps (Absolute Timeline, Can Overlap)")]
    public List<TextStep> steps = new List<TextStep>();

    [Header("Player Lock (Disable during intro)")]
    [Tooltip("Drag PlayerMove, MouseLook, etc. These will be disabled at intro start, re-enabled when unlocked.")]
    public Behaviour[] disableBehavioursOnStart;

    [Header("Unlock Control")]
    public UnlockMode unlockMode = UnlockMode.AfterAllTextsFinished;
    [Tooltip("Only used when UnlockMode = AtExactTime. Absolute seconds from intro start.")]
    [Min(0f)] public float unlockAtTime = 0f;

    [Header("Options")]
    public bool playOnStart = true;
    public bool useUnscaledTime = true;

    [Header("Skip Rules")]
    [Tooltip("从 Options 返回时跳过 Intro（解决你现在的问题）。")]
    public bool skipIfReturningFromOptions = true;

    [Tooltip("Intro 只播一次（同一存档/同一台机器）。")]
    public bool playOnlyOnce = false;

    private float _introStartTime;
    private bool _skipIntroThisLoad;

    private void Awake()
    {
        _introStartTime = Now();

        // 先把 UI 全部初始化到“干净状态”
        if (overlayGroup) overlayGroup.alpha = 0f; // 默认不要黑屏（除非要播intro再设为1）

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

        // ✅ 判断是否要跳过 Intro
        _skipIntroThisLoad = ShouldSkipIntro();

        if (_skipIntroThisLoad)
        {
            // 跳过：确保玩家可控
            SetBehavioursEnabled(true);
            return;
        }

        // 不跳过：进入 intro 的初始状态（黑屏 + 锁玩家）
        if (overlayGroup) overlayGroup.alpha = 1f; // start black
        SetBehavioursEnabled(false);
    }

    private void Start()
    {
        if (!playOnStart) return;
        if (_skipIntroThisLoad) return;

        StartCoroutine(PlayIntro());
    }

    private bool ShouldSkipIntro()
    {
        // 1) 从 Options 返回：跳过（并清掉flag，避免影响下次正常进入）
        if (skipIfReturningFromOptions && PlayerPrefs.GetInt(KEY_RETURNING_FROM_OPTIONS, 0) == 1)
        {
            PlayerPrefs.DeleteKey(KEY_RETURNING_FROM_OPTIONS);
            PlayerPrefs.Save();
            return true;
        }

        // 2) 只播一次：已经播过就跳过
        if (playOnlyOnce && PlayerPrefs.GetInt(KEY_INTRO_DONE, 0) == 1)
            return true;

        return false;
    }

    private IEnumerator PlayIntro()
    {
        if (!overlayGroup)
        {
            Debug.LogError("Scene1IntroLockAndText_V2: Missing overlayGroup. Assign FadeOverlay CanvasGroup in Inspector.");
            yield break;
        }

        // 1) Hold fully black
        if (enterHoldBlack > 0f)
            yield return WaitSeconds(enterHoldBlack);

        // 2) Fade OUT black overlay: alpha 1 -> 0
        yield return Fade(overlayGroup, 1f, 0f, overlayFadeOutBlack);

        // 3) Start all text steps concurrently on absolute timeline
        float latestEnd = 0f;

        if (steps != null && steps.Count > 0)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                var s = steps[i];
                if (s == null) continue;

                if (!s.text || !s.textGroup)
                {
                    Debug.LogError($"Scene1IntroLockAndText_V2: Step {i} missing TMP_Text or CanvasGroup.");
                    continue;
                }

                if (s.EndTime > latestEnd) latestEnd = s.EndTime;

                StartCoroutine(PlayStep(s));
            }
        }

        // 4) Unlock timing
        float unlockTime = 0f;
        switch (unlockMode)
        {
            case UnlockMode.AfterAllTextsFinished:
                unlockTime = latestEnd;
                break;
            case UnlockMode.AtExactTime:
                unlockTime = unlockAtTime;
                break;
        }

        if (unlockTime > 0f)
            yield return WaitUntilElapsed(unlockTime);

        // 5) Unlock player behaviours
        SetBehavioursEnabled(true);

        // ✅ 记住已经播过（如果你启用了 playOnlyOnce）
        if (playOnlyOnce)
        {
            PlayerPrefs.SetInt(KEY_INTRO_DONE, 1);
            PlayerPrefs.Save();
        }
    }

    private IEnumerator PlayStep(TextStep s)
    {
        yield return WaitUntilElapsed(s.sceneDelay);

        s.text.text = s.content ?? "";

        yield return Fade(s.textGroup, 0f, 1f, s.fadeIn);

        if (s.hold > 0f)
            yield return WaitSeconds(s.hold);

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
        return Now() - _introStartTime;
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
