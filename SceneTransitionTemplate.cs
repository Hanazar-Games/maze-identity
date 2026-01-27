using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTemplate : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup overlayGroup;   // FadeOverlay 的 CanvasGroup
    public CanvasGroup textGroup;      // Title 的 CanvasGroup

    [Header("Enter Sequence")]
    public float fadeInDuration = 1.0f;      // 黑 -> 透明（场景渐显）
    public float enterHoldBlack = 0.0f;      // 进入后黑屏停留

    [Header("Text Sequence")]
    public float textFadeIn = 0.6f;
    public float textHold = 0.8f;
    public float textFadeOut = 0.6f;

    [Header("Exit Sequence")]
    public float fadeOutDuration = 1.0f;     // 透明 -> 黑（场景渐隐）
    public float exitHoldBlack = 0.0f;       // 黑屏停留（可用于加载下一场景前）

    [Header("Options")]
    public bool playOnStart = true;
    public bool blockInputWhileFading = true;

    void Reset()
    {
        // 尽量自动找（可选）
        var groups = GetComponentsInChildren<CanvasGroup>(true);
        if (groups.Length >= 2)
        {
            overlayGroup = groups[0];
            textGroup = groups[1];
        }
    }

    void Start()
    {
        if (playOnStart)
            StartCoroutine(EnterSequence());
    }

    public IEnumerator EnterSequence()
    {
        if (!overlayGroup || !textGroup) yield break;

        // 初始：强制黑屏 + 文字不可见
        overlayGroup.alpha = 1f;
        textGroup.alpha = 0f;

        SetBlockInput(true);

        if (enterHoldBlack > 0f)
            yield return new WaitForSeconds(enterHoldBlack);

        // 黑 -> 透明（场景渐显）
        yield return Fade(overlayGroup, 1f, 0f, fadeInDuration);

        // Text 渐显 -> 停留 -> 渐隐
        yield return Fade(textGroup, 0f, 1f, textFadeIn);
        if (textHold > 0f) yield return new WaitForSeconds(textHold);
        yield return Fade(textGroup, 1f, 0f, textFadeOut);

        SetBlockInput(false);
    }

    public IEnumerator ExitSequence()
    {
        if (!overlayGroup) yield break;

        SetBlockInput(true);

        // 透明 -> 黑（场景渐隐）
        yield return Fade(overlayGroup, 0f, 1f, fadeOutDuration);

        if (exitHoldBlack > 0f)
            yield return new WaitForSeconds(exitHoldBlack);
    }

    /// <summary>
    /// 退出并切换到下一场景（建议用这个对接你关卡 trigger）
    /// </summary>
    public void FadeOutAndLoad(string sceneName)
    {
        StartCoroutine(FadeOutAndLoadRoutine(sceneName));
    }

    private IEnumerator FadeOutAndLoadRoutine(string sceneName)
    {
        yield return ExitSequence();

        // 异步加载更稳
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
            yield return null;
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
            t += Time.unscaledDeltaTime; // 不受 Time.timeScale 影响
            float p = Mathf.Clamp01(t / duration);
            g.alpha = Mathf.Lerp(from, to, p);
            yield return null;
        }
        g.alpha = to;
    }

    private void SetBlockInput(bool block)
    {
        if (!blockInputWhileFading) return;
        // overlay 的 RaycastTarget 决定是否挡输入；CanvasGroup.blocksRaycasts 也可以
        overlayGroup.blocksRaycasts = block;
        overlayGroup.interactable = block;
    }
}
