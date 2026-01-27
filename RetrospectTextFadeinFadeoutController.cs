using System.Collections;
using TMPro;
using UnityEngine;

public class TriggerHintFader : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("拖入提示文字 TMP_Text")]
    public TMP_Text hintText;

    [Tooltip("拖入控制透明度的 CanvasGroup（建议在文字物体上）")]
    public CanvasGroup hintGroup;

    [Header("Text Content")]
    [TextArea(1, 3)]
    public string message = "Hold G to enter scene";

    [Header("Fade Settings")]
    [Min(0f)] public float fadeInSeconds = 0.25f;
    [Min(0f)] public float fadeOutSeconds = 0.25f;

    [Header("Optional")]
    public bool useUnscaledTime = true;
    public bool debugLogs = false;

    private Coroutine _fadeCo;

    private void Reset()
    {
        // 尝试自动找
        hintText = GetComponentInChildren<TMP_Text>(true);
        hintGroup = GetComponentInChildren<CanvasGroup>(true);
    }

    private void Awake()
    {
        if (hintText != null) hintText.text = message;

        // 默认隐藏
        if (hintGroup != null)
        {
            hintGroup.alpha = 0f;
            hintGroup.interactable = false;
            hintGroup.blocksRaycasts = false;
        }
    }

    public void Show()
    {
        if (hintText != null) hintText.text = message;

        if (debugLogs) Debug.Log($"[TriggerHintFader] Show() on {name}");
        StartFade(1f, fadeInSeconds);
    }

    public void Hide()
    {
        if (debugLogs) Debug.Log($"[TriggerHintFader] Hide() on {name}");
        StartFade(0f, fadeOutSeconds);
    }

    private void StartFade(float targetAlpha, float duration)
    {
        if (hintGroup == null) return;

        if (_fadeCo != null) StopCoroutine(_fadeCo);
        _fadeCo = StartCoroutine(FadeRoutine(targetAlpha, duration));
    }

    private IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        if (hintGroup == null) yield break;

        float startAlpha = hintGroup.alpha;

        if (duration <= 0f)
        {
            hintGroup.alpha = targetAlpha;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            hintGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, p);
            yield return null;
        }

        hintGroup.alpha = targetAlpha;
    }
}
