using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene6HoldGTransitionController : MonoBehaviour
{
    [Header("Enable")]
    public bool enable = true;

    [Header("Input")]
    public KeyCode holdKey = KeyCode.G;
    [Min(0.1f)] public float holdSecondsToTrigger = 2f;

    [Header("After Trigger")]
    [Min(0f)] public float delayBeforeLoad = 3f;   // 触发后等待/过渡总时长（可调）
    public string nextSceneName = "SceneTransitions6to7";

    [Header("Fade Overlay (CanvasGroup)")]
    public CanvasGroup overlayGroup;
    public bool forceOverlayAlphaOnAwake = true;
    [Range(0f, 1f)] public float overlayStartAlpha = 0f;
    [Range(0f, 1f)] public float overlayTargetAlpha = 1f;

    [Header("Linked Text Fade Out (only when triggered)")]
    public CanvasGroup linkedTextGroup;            // Text2 的 CanvasGroup
    public bool fadeTextDuringTransition = true;   // ✅ 复选框
    [Min(0f)] public float textFadeOutDuration = 2f; // ✅ 你要的 2秒可调
    public bool keepTextVisibleUntilTriggered = true; // ✅ 开场不改它，直到触发才管

    [Header("Options")]
    public bool useUnscaledTime = true;
    public bool holdUsesUnscaledTime = true;

    private float _holdTimer = 0f;
    private bool _triggered = false;

    private void Awake()
    {
        if (!enable) return;

        if (overlayGroup && forceOverlayAlphaOnAwake)
            overlayGroup.alpha = overlayStartAlpha;

        // 不要开场把文字弄没：让 Intro 脚本控制它
        // 如果你确实想强制开场保持显示，可在这里处理，但默认不做
    }

    private void Update()
    {
        if (!enable || _triggered) return;

        float dt = holdUsesUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (Input.GetKey(holdKey))
        {
            _holdTimer += dt;
            if (_holdTimer >= holdSecondsToTrigger)
            {
                _triggered = true;
                StartCoroutine(TransitionRoutine());
            }
        }
        else
        {
            _holdTimer = 0f;
        }
    }

    private IEnumerator TransitionRoutine()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("Scene6HoldGTransitionController: nextSceneName is empty.");
            yield break;
        }

        // Overlay 起点固定
        if (overlayGroup)
            overlayGroup.alpha = overlayStartAlpha;

        // Text 起点：如果你希望“直到触发都保持可见”，就取当前alpha作为起点
        float textFrom = 1f;
        if (linkedTextGroup)
        {
            textFrom = keepTextVisibleUntilTriggered ? linkedTextGroup.alpha : 1f;
        }

        // 先让 overlay 在 delayBeforeLoad 内变黑
        // 同时：Text2 在 textFadeOutDuration 内淡出（可比 overlay 更短/更长）
        float total = Mathf.Max(0f, delayBeforeLoad);

        if (total > 0f)
        {
            yield return FadeOverlayAndText(total, textFrom);
        }
        else
        {
            if (overlayGroup) overlayGroup.alpha = overlayTargetAlpha;
            if (linkedTextGroup && fadeTextDuringTransition) linkedTextGroup.alpha = 0f;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeOverlayAndText(float overlayDuration, float textFrom)
    {
        float t = 0f;

        float oFrom = overlayGroup ? overlayGroup.alpha : overlayStartAlpha;
        float oTo = overlayTargetAlpha;

        float textDur = Mathf.Max(0f, textFadeOutDuration);

        while (t < overlayDuration)
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            t += dt;

            float pOverlay = Mathf.Clamp01(t / overlayDuration);

            if (overlayGroup)
                overlayGroup.alpha = Mathf.Lerp(oFrom, oTo, pOverlay);

            if (linkedTextGroup && fadeTextDuringTransition)
            {
                // Text 在 textDur 内淡出；超过后保持0
                float pText = (textDur <= 0f) ? 1f : Mathf.Clamp01(t / textDur);
                linkedTextGroup.alpha = Mathf.Lerp(textFrom, 0f, pText);
            }

            yield return null;
        }

        if (overlayGroup) overlayGroup.alpha = oTo;
        if (linkedTextGroup && fadeTextDuringTransition) linkedTextGroup.alpha = 0f;
    }
}
