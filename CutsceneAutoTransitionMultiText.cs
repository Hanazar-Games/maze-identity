using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneAutoTransitionMultiText : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup overlayGroup;     // FadeOverlay CanvasGroup
    public CanvasGroup textGroup;        // Title CanvasGroup
    public TMP_Text titleText;           // Title 的 TMP 组件（用来改文字）

    [Header("Text Lines (in order)")]
    [TextArea(1, 4)]
    public string[] lines;

    [Header("Sequence Timings")]
    public float enterHoldBlack = 0.0f;
    public float fadeInDuration = 1.0f;

    // 每条文本统一的节奏
    public float textFadeIn = 0.6f;
    public float textHold = 1.0f;
    public float textFadeOut = 0.6f;

    // 每条文本之间的间隔（可为0）
    public float gapBetweenLines = 0.0f;

    public float fadeOutDuration = 1.0f;
    public float exitHoldBlack = 0.0f;

    [Header("Next Scene")]
    public string nextSceneName;

    [Header("Options")]
    public bool playOnStart = true;

    private void Awake()
    {
        if (overlayGroup) overlayGroup.alpha = 1f; // 初始黑
        if (textGroup) textGroup.alpha = 0f;       // 文本隐藏
        if (titleText && lines != null && lines.Length > 0)
            titleText.text = lines[0];
    }

    private void Start()
    {
        if (playOnStart)
            StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        if (!overlayGroup || !textGroup || !titleText)
        {
            Debug.LogError("CutsceneAutoTransitionMultiText: Missing references (overlay/textGroup/titleText).");
            yield break;
        }

        // 进场黑屏停留
        if (enterHoldBlack > 0f)
            yield return new WaitForSeconds(enterHoldBlack);

        // 黑 -> 透明
        yield return Fade(overlayGroup, 1f, 0f, fadeInDuration);

        // 依次播放多条文本
        if (lines != null && lines.Length > 0)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                titleText.text = lines[i];

                yield return Fade(textGroup, 0f, 1f, textFadeIn);
                if (textHold > 0f) yield return new WaitForSeconds(textHold);
                yield return Fade(textGroup, 1f, 0f, textFadeOut);

                if (gapBetweenLines > 0f && i < lines.Length - 1)
                    yield return new WaitForSeconds(gapBetweenLines);
            }
        }

        // 透明 -> 黑
        yield return Fade(overlayGroup, 0f, 1f, fadeOutDuration);

        if (exitHoldBlack > 0f)
            yield return new WaitForSeconds(exitHoldBlack);

        // 自动切场景
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("CutsceneAutoTransitionMultiText: nextSceneName is empty.");
            yield break;
        }

        SceneManager.LoadScene(nextSceneName);
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
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            g.alpha = Mathf.Lerp(from, to, p);
            yield return null;
        }

        g.alpha = to;
    }
}
