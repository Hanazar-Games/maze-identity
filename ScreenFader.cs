using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MI_ScreenFaderButtonHook : MonoBehaviour
{
    [Header("Overlay (choose one)")]
    [Tooltip("推荐：CanvasGroup（更稳）。")]
    public CanvasGroup fadeGroup;

    [Tooltip("可选：Image（也可以两者都拖）。")]
    public Image fadeImage;

    [Header("Hook Buttons")]
    [Tooltip("把需要“点击先Fade再执行”的按钮全部拖进来。")]
    public List<Button> buttons = new List<Button>();

    [Header("Fade Settings")]
    [Min(0f)] public float fadeToBlackSeconds = 2f;
    [Min(0f)] public float holdBlackSeconds = 0f;

    [Tooltip("使用不受 Time.timeScale 影响的时间（推荐勾上，暂停时也能fade）。")]
    public bool useUnscaledTime = true;

    [Header("Behaviour")]
    [Tooltip("进入场景时是否从黑淡出（可选）。")]
    public bool fadeInOnStart = false;
    [Min(0f)] public float fadeInSeconds = 0.5f;
    [Min(0f)] public float fadeInHoldBlackSeconds = 0f;

    [Header("Safety")]
    [Tooltip("Fade期间禁用按钮交互，防止连点。")]
    public bool disableButtonsWhileFading = true;

    [Tooltip("遮罩可见时阻挡射线，防止点穿UI。")]
    public bool blockRaycastsWhenVisible = true;

    private bool _busy;
    private bool _hooked;

    private class Hook
    {
        public Button button;
        public Button.ButtonClickedEvent originalOnClick;
    }

    private readonly List<Hook> _hooks = new List<Hook>();

    private void Awake()
    {
        EnsureOverlayAssigned();

        // 默认不挡UI
        if (!fadeInOnStart)
        {
            SetAlpha(0f);
            SetOverlayActive(false);
        }
        else
        {
            // 先黑再淡出
            SetOverlayActive(true);
            SetAlpha(1f);
        }
    }

    private void Start()
    {
        HookButtonsOnce();

        if (fadeInOnStart)
        {
            StopAllCoroutines();
            StartCoroutine(FadeInRoutine());
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

            // 保存原始事件
            var hook = new Hook
            {
                button = b,
                originalOnClick = b.onClick
            };

            // 替换成新的事件（先Fade，再Invoke原事件）
            b.onClick = new Button.ButtonClickedEvent();
            b.onClick.AddListener(() =>
            {
                if (_busy) return;
                StartCoroutine(FadeOutThenInvoke(hook.originalOnClick));
            });

            _hooks.Add(hook);
        }
    }

    private void UnhookButtons()
    {
        foreach (var h in _hooks)
        {
            if (h != null && h.button != null && h.originalOnClick != null)
            {
                h.button.onClick = h.originalOnClick;
            }
        }

        _hooks.Clear();
        _hooked = false;
    }

    // =========================
    // Fade routines
    // =========================
    private IEnumerator FadeOutThenInvoke(Button.ButtonClickedEvent originalEvent)
    {
        _busy = true;
        if (disableButtonsWhileFading) SetButtonsInteractable(false);

        // 透明 -> 黑
        SetOverlayActive(true);
        yield return Fade(GetAlpha(), 1f, fadeToBlackSeconds);

        if (holdBlackSeconds > 0f)
            yield return WaitSeconds(holdBlackSeconds);

        // 执行原按钮逻辑（LoadScene / Quit / OpenPanel）
        if (originalEvent != null)
            originalEvent.Invoke();

        // 注意：一般会切场景，所以这里不做自动淡回
        _busy = false;
    }

    private IEnumerator FadeInRoutine()
    {
        _busy = true;
        if (disableButtonsWhileFading) SetButtonsInteractable(false);

        SetOverlayActive(true);
        SetAlpha(1f);

        if (fadeInHoldBlackSeconds > 0f)
            yield return WaitSeconds(fadeInHoldBlackSeconds);

        yield return Fade(1f, 0f, fadeInSeconds);

        SetAlpha(0f);
        SetOverlayActive(false);

        if (disableButtonsWhileFading) SetButtonsInteractable(true);
        _busy = false;
    }

    private IEnumerator Fade(float from, float to, float seconds)
    {
        float d = Mathf.Max(0.0001f, seconds);
        float t = 0f;

        SetAlpha(from);

        while (t < d)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float a = Mathf.Lerp(from, to, Mathf.Clamp01(t / d));
            SetAlpha(a);
            yield return null;
        }

        SetAlpha(to);
    }

    private IEnumerator WaitSeconds(float seconds)
    {
        if (seconds <= 0f) yield break;
        if (useUnscaledTime) yield return new WaitForSecondsRealtime(seconds);
        else yield return new WaitForSeconds(seconds);
    }

    // =========================
    // Overlay helpers
    // =========================
    private void EnsureOverlayAssigned()
    {
        if (fadeGroup == null && fadeImage == null)
            Debug.LogWarning("[MI_ScreenFaderButtonHook] No fadeGroup/fadeImage assigned. Fade will not be visible.");
    }

    private void SetOverlayActive(bool active)
    {
        if (fadeGroup != null) fadeGroup.gameObject.SetActive(active);
        else if (fadeImage != null) fadeImage.gameObject.SetActive(active);
    }

    private float GetAlpha()
    {
        if (fadeGroup != null) return fadeGroup.alpha;
        if (fadeImage != null) return fadeImage.color.a;
        return 0f;
    }

    private void SetAlpha(float a)
    {
        a = Mathf.Clamp01(a);

        if (fadeGroup != null)
        {
            fadeGroup.alpha = a;
            if (blockRaycastsWhenVisible)
                fadeGroup.blocksRaycasts = a > 0.001f;
        }

        if (fadeImage != null)
        {
            var c = fadeImage.color;
            c.a = a;
            fadeImage.color = c;

            if (blockRaycastsWhenVisible)
                fadeImage.raycastTarget = a > 0.001f;
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
