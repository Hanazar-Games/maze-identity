using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneAutoTransition : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup overlayGroup;   // FadeOverlay CanvasGroup (black)
    public CanvasGroup textGroup;      // Title CanvasGroup

    [Header("Sequence Timings")]
    public float enterHoldBlack = 0.2f;     // 进场黑屏停留
    public float fadeInDuration = 1.0f;    // 黑 -> 透明（画面渐显）

    public float textFadeIn = 0.6f;        // 字体渐显
    public float textHold = 1.2f;          // 字体停留
    public float textFadeOut = 0.6f;       // 字体渐隐

    public float fadeOutDuration = 1.0f;   // 透明 -> 黑（场景渐隐）
    public float exitHoldBlack = 0.2f;     // 黑屏停留（给加载留缓冲）

    [Header("Next Scene")]
    public string nextSceneName;           // 下一场景名（必须在 Build Settings）

    [Header("Options")]
    public bool playOnStart = true;

    private void Awake()
    {
        // 强制初始状态：一进来先黑，文字不可见
        if (overlayGroup) overlayGroup.alpha = 1f;
        if (textGroup) textGroup.alpha = 0f;

        // 可选：防止TimeScale影响
        Time.timeScale = 1f;
    }

    private void Start()
    {
        if (playOnStart)
            StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        if (!overlayGroup || !textGroup)
        {
            Debug.LogError("CutsceneAutoTransition: Missing CanvasGroup references.");
            yield break;
        }

        // 1) 进场黑屏停留
        if (enterHoldBlack > 0f)
            yield return new WaitForSeconds(enterHoldBlack);

        // 2) 黑 -> 透明（画面渐显）
        yield return Fade(overlayGroup, 1f, 0f, fadeInDuration);

        // 3) 字体渐显 -> 停留 -> 渐隐
        yield return Fade(textGroup, 0f, 1f, textFadeIn);
        if (textHold > 0f) yield return new WaitForSeconds(textHold);
        yield return Fade(textGroup, 1f, 0f, textFadeOut);

        // 4) 透明 -> 黑（场景渐隐）
        yield return Fade(overlayGroup, 0f, 1f, fadeOutDuration);

        // 5) 黑屏停留
        if (exitHoldBlack > 0f)
            yield return new WaitForSeconds(exitHoldBlack);

        // 6) 自动切下一场景
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("CutsceneAutoTransition: nextSceneName is empty.");
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
