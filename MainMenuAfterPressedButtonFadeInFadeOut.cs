using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuAfterPressedButtonFadeInFadeOut : MonoBehaviour
{
    [Header("Fade Overlay (choose one)")]
    [Tooltip("推荐：拖 FadeOverlay 上的 CanvasGroup（更稳）。")]
    public CanvasGroup fadeGroup;

    [Tooltip("可选：拖 FadeOverlay 的 Image（也可以两者都拖）。")]
    public Image fadeImage;

    [Header("Buttons to hook (MainMenu only)")]
    [Tooltip("把 MainMenu 里所有需要“点了先黑场”的按钮拖进来。")]
    public List<Button> buttons = new List<Button>();

    [Header("Timing")]
    [Tooltip("点击按钮后：淡到黑的时间（秒）。")]
    [Min(0f)] public float fadeToBlackSeconds = 2f;

    [Tooltip("进入 MainMenu 时：从黑淡出到透明的时间（秒）。")]
    [Min(0f)] public float enterFadeInSeconds = 1.5f;

    [Tooltip("进入 MainMenu 时：先保持黑场多少秒再淡出（秒）。")]
    [Min(0f)] public float enterHoldBlackSeconds = 0f;

    [Tooltip("使用不受 Time.timeScale 影响的时间（推荐勾上）。")]
    public bool useUnscaledTime = true;

    [Header("Enter Behavior")]
    [Tooltip("不管从哪里进 MainMenu，一进来就做 FadeIn。")]
    public bool fadeInOnMenuEnter = true;

    [Tooltip("进入时强制先黑（alpha=1），再淡出。")]
    public bool forceBlackOnEnter = true;

    [Header("Safety")]
    [Tooltip("淡入/淡出期间禁用按钮交互，防止连点。")]
    public bool disableButtonsWhileFading = true;

    // ===== internal =====
    private bool _isFading = false;
    private bool _hooked = false;

    private class Hook
    {
        public Button button;
        public Button.ButtonClickedEvent originalOnClick;
    }

    private readonly List<Hook> _hooks = new List<Hook>();

    private void Awake()
    {
        EnsureOverlayAssigned();
    }

    private void Start()
    {
        HookButtonsOnce();

        if (fadeInOnMenuEnter)
        {
            StopAllCoroutines();
            StartCoroutine(EnterFadeInRoutine());
        }
        else
        {
            SetOverlayActive(false);
            SetOverlayAlpha(0f);
            SetButtonsInteractable(true);
        }
    }

    private void OnDestroy()
    {
        UnhookButtons();
    }

    [ContextMenu("Refresh Hook Buttons")]
    public void RefreshHookButtons()
    {
        UnhookButtons();
        HookButtonsOnce();
    }

    /// <summary>
    /// 给外部脚本用：先 FadeToBlack，再执行 action（例如 LoadScene）。
    /// 这就是你 MainMenuContinueController 里需要的 FadeThen。
    /// </summary>
    public void FadeThen(Action action)
    {
        if (_isFading) return;
        StartCoroutine(FadeToBlackThenAction(action));
    }

    // =========================
    // Hook logic
    // =========================
    private void HookButtonsOnce()
    {
        if (_hooked) return;
        _hooked = true;

        _hooks.Clear();

        foreach (var b in buttons)
        {
            if (b == null) continue;

            var hook = new Hook
            {
                button = b,
                originalOnClick = b.onClick
            };

            // 替换按钮事件为新的事件
            b.onClick = new Button.ButtonClickedEvent();

            // 新事件：先黑场，再 Invoke 原事件
            b.onClick.AddListener(() =>
            {
                if (_isFading) return;
                StartCoroutine(FadeToBlackThenInvoke(hook.originalOnClick));
            });

            _hooks.Add(hook);
        }
    }

    private void UnhookButtons()
    {
        foreach (var h in _hooks)
        {
            if (h != null && h.button != null && h.originalOnClick != null)
                h.button.onClick = h.originalOnClick;
        }
        _hooks.Clear();
        _hooked = false;
    }

    // =========================
    // Fade routines
    // =========================
    private IEnumerator EnterFadeInRoutine()
    {
        _isFading = true;

        if (disableButtonsWhileFading) SetButtonsInteractable(false);

        // 进入时先黑
        SetOverlayActive(true);
        if (forceBlackOnEnter) SetOverlayAlpha(1f);

        // 保持黑场
        if (enterHoldBlackSeconds > 0f)
        {
            if (useUnscaledTime) yield return new WaitForSecondsRealtime(enterHoldBlackSeconds);
            else yield return new WaitForSeconds(enterHoldBlackSeconds);
        }

        // 黑 -> 透明
        float duration = Mathf.Max(0.0001f, enterFadeInSeconds);
        float t = 0f;
        float startA = forceBlackOnEnter ? 1f : GetOverlayAlpha();

        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float a = Mathf.Lerp(startA, 0f, Mathf.Clamp01(t / duration));
            SetOverlayAlpha(a);
            yield return null;
        }

        SetOverlayAlpha(0f);
        SetOverlayActive(false);

        if (disableButtonsWhileFading) SetButtonsInteractable(true);

        _isFading = false;
    }

    private IEnumerator FadeToBlackThenInvoke(Button.ButtonClickedEvent originalEvent)
    {
        _isFading = true;
        if (disableButtonsWhileFading) SetButtonsInteractable(false);

        yield return FadeToBlackRoutine();

        // 到黑场后执行原按钮逻辑
        if (originalEvent != null) originalEvent.Invoke();

        _isFading = false;
    }

    private IEnumerator FadeToBlackThenAction(Action action)
    {
        _isFading = true;
        if (disableButtonsWhileFading) SetButtonsInteractable(false);

        yield return FadeToBlackRoutine();

        action?.Invoke();

        _isFading = false;
    }

    private IEnumerator FadeToBlackRoutine()
    {
        SetOverlayActive(true);

        float startA = GetOverlayAlpha();
        float duration = Mathf.Max(0.0001f, fadeToBlackSeconds);
        float t = 0f;

        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float a = Mathf.Lerp(startA, 1f, Mathf.Clamp01(t / duration));
            SetOverlayAlpha(a);
            yield return null;
        }

        SetOverlayAlpha(1f);
    }

    // =========================
    // Overlay utilities
    // =========================
    private void EnsureOverlayAssigned()
    {
        if (fadeGroup == null && fadeImage == null)
            Debug.LogWarning("[MainMenuFade] No fadeGroup or fadeImage assigned. Fade will not work.");
    }

    private void SetOverlayActive(bool active)
    {
        if (fadeGroup != null) fadeGroup.gameObject.SetActive(active);
        else if (fadeImage != null) fadeImage.gameObject.SetActive(active);
    }

    private float GetOverlayAlpha()
    {
        if (fadeGroup != null) return fadeGroup.alpha;
        if (fadeImage != null) return fadeImage.color.a;
        return 0f;
    }

    private void SetOverlayAlpha(float a)
    {
        a = Mathf.Clamp01(a);

        if (fadeGroup != null)
        {
            fadeGroup.alpha = a;
            // 黑场时挡住射线，防止“点穿”
            fadeGroup.blocksRaycasts = (a > 0.001f);
        }

        if (fadeImage != null)
        {
            var c = fadeImage.color;
            c.a = a;
            fadeImage.color = c;
            fadeImage.raycastTarget = (a > 0.001f);
        }
    }

    private void SetButtonsInteractable(bool on)
    {
        if (!disableButtonsWhileFading) return;

        foreach (var b in buttons)
        {
            if (b != null) b.interactable = on;
        }
    }
}
