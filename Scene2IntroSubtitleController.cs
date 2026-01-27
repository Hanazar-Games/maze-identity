using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scene2IntroSubtitleController : MonoBehaviour
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

        [Header("Timeline (Absolute from Intro Start)")]
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

    // ===================== Options Return Skip =====================

    [Header("Skip Intro When Returning From Options")]
    [Tooltip("从 Options 返回时跳过 Intro（黑幕+字幕）。建议开启。")]
    public bool skipIntroWhenReturningFromOptions = true;

    [Tooltip("你现在 Options 系统统一用的返回标记 key（推荐：MI_ReturningFromOptions）。")]
    public string returningFromOptionsKey = "MI_ReturningFromOptions";

    [Tooltip("兼容旧 key：如果你项目里旧脚本还在用这个 key，也会一并检查并清除。")]
    public string legacyReturningKey = "MI_ReturningFromOptions_Legacy";

    [Tooltip("调试输出：打印是否跳过 Intro。")]
    public bool debugLogs = true;

    // ===============================================================

    [Header("Overlay (Black Screen)")]
    [Tooltip("CanvasGroup on a full-screen black Image. Alpha=1 means black.")]
    public CanvasGroup overlayGroup;

    [Tooltip("Hold fully black at the very beginning (seconds).")]
    [Min(0f)] public float enterHoldBlack = 0f;

    [Tooltip("Fade OUT the black overlay first (alpha 1 -> 0).")]
    [Min(0f)] public float overlayFadeOutBlack = 1.0f;

    [Tooltip("If true, text timeline starts AFTER overlayFadeOutBlack finishes. (Recommended)")]
    public bool startTextAfterOverlayFadeOut = true;

    [Header("Text Steps (Absolute Timeline, Can Overlap)")]
    public List<TextStep> steps = new List<TextStep>();

    [Header("Player Lock (Optional)")]
    public Behaviour[] disableBehavioursOnStart;

    [Header("Unlock Control")]
    public UnlockMode unlockMode = UnlockMode.AfterAllTextsFinished;
    [Min(0f)] public float unlockAtTime = 0f;

    [Header("Options")]
    public bool playOnStart = true;
    public bool useUnscaledTime = true;

    private float _introStartTime;
    private float _textTimelineOffset;
    private Coroutine _introCo;
    private bool _skipped;

    private void Awake()
    {
        _introStartTime = Now();

        // Start fully black (fadeoutfirst workflow)
        if (overlayGroup) overlayGroup.alpha = 1f;

        // Hide texts at start and prefill
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

        // Lock immediately if assigned
        SetBehavioursEnabled(false);
    }

    private void Start()
    {
        // 关键：从 Options 返回则跳过 Intro
        if (skipIntroWhenReturningFromOptions && IsReturningFromOptionsAndConsumeFlag())
        {
            _skipped = true;

            if (debugLogs)
                Debug.Log("[Scene2IntroSubtitleController] Skip intro: returning from Options.");

            SkipIntroImmediate();
            return;
        }

        if (playOnStart)
            _introCo = StartCoroutine(PlayIntro());
        else
            SkipIntroImmediate(); // playOnStart 关掉就当作直接跳过
    }

    private IEnumerator PlayIntro()
    {
        if (!overlayGroup)
        {
            Debug.LogError("Scene2IntroSubtitleController: Missing overlayGroup.");
            yield break;
        }

        // 1) Hold black
        if (enterHoldBlack > 0f)
            yield return WaitSeconds(enterHoldBlack);

        // 2) Fade out black first (black -> visible)
        yield return Fade(overlayGroup, 1f, 0f, overlayFadeOutBlack);

        // 3) Decide text timeline offset
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
                    Debug.LogError($"Scene2IntroSubtitleController: Step {i} missing TMP_Text or CanvasGroup.");
                    continue;
                }

                if (s.EndTime > latestEnd) latestEnd = s.EndTime;

                StartCoroutine(PlayStep(s));
            }
        }

        // 5) Unlock timing
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

        // Unlock time is relative to text timeline start; convert to absolute by adding offset
        float absoluteUnlock = _textTimelineOffset + unlockTime;

        if (absoluteUnlock > 0f)
            yield return WaitUntilElapsed(absoluteUnlock);

        // 6) Unlock
        SetBehavioursEnabled(true);
    }

    private IEnumerator PlayStep(TextStep s)
    {
        float absoluteStart = _textTimelineOffset + s.sceneDelay;
        yield return WaitUntilElapsed(absoluteStart);

        if (s.text) s.text.text = s.content ?? "";

        if (s.textGroup)
        {
            yield return Fade(s.textGroup, 0f, 1f, s.fadeIn);

            if (s.hold > 0f)
                yield return WaitSeconds(s.hold);

            yield return Fade(s.textGroup, 1f, 0f, s.fadeOut);
        }
    }

    // ===================== Skip Intro Logic =====================

    private void SkipIntroImmediate()
    {
        // 停掉可能的协程（保险）
        if (_introCo != null) StopCoroutine(_introCo);
        StopAllCoroutines();

        // 直接把黑幕和字幕状态设置为“Intro 完成后”
        if (overlayGroup)
            overlayGroup.alpha = 0f;

        if (steps != null)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                var s = steps[i];
                if (s == null) continue;

                if (s.textGroup) s.textGroup.alpha = 0f;
            }
        }

        // 立刻解锁玩家
        SetBehavioursEnabled(true);
    }

    private bool IsReturningFromOptionsAndConsumeFlag()
    {
        // 1) 优先检查你现在统一的 key
        int v = PlayerPrefs.GetInt(returningFromOptionsKey, 0);

        // 2) 兼容旧 key（如果你项目里曾经分散用过）
        if (v != 1 && !string.IsNullOrEmpty(legacyReturningKey))
            v = PlayerPrefs.GetInt(legacyReturningKey, 0);

        // 3) 兼容你以前写过的 Scene1IntroLockAndText_V2.KEY_RETURNING_FROM_OPTIONS（如果类存在且 key 可访问）
        //    这里不直接引用类名，避免你改名/删类导致编译报错，所以只做“你可以手动填 legacyReturningKey”的策略。
        //    若你确定你当前用的是 Scene1IntroLockAndText_V2.KEY_RETURNING_FROM_OPTIONS，请把 legacyReturningKey 设置成那个 key 的实际字符串。

        if (v == 1)
        {
            // 用完就清，确保只 skip 一次
            PlayerPrefs.SetInt(returningFromOptionsKey, 0);
            if (!string.IsNullOrEmpty(legacyReturningKey))
                PlayerPrefs.SetInt(legacyReturningKey, 0);

            PlayerPrefs.Save();
            return true;
        }

        return false;
    }

    // ============================================================

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
