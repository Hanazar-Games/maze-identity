using System.Collections;
using TMPro;
using UnityEngine;

public class IntroFourTextTimelineController : MonoBehaviour
{
    [Header("Enable")]
    public bool enable = true;

    [Header("Options")]
    public bool useUnscaledTime = true;

    [Header("Startup Visibility")]
    public bool forceHiddenOnStart = true;

    // ===================== TEXT 1 =====================
    [Header("Text 1 (Absolute from Scene Start)")]
    public TMP_Text text1;
    public CanvasGroup group1;
    [TextArea] public string content1;
    public float delay1;
    public float fadeIn1 = 1f;
    public float hold1 = 2f;
    public float fadeOut1 = 1f;

    // ===================== TEXT 2 =====================
    [Header("Text 2 (Absolute from Scene Start)")]
    public TMP_Text text2;
    public CanvasGroup group2;
    [TextArea] public string content2;
    public float delay2;
    public float fadeIn2 = 1f;
    public float hold2 = 2f;
    public float fadeOut2 = 1f;

    // ===================== TEXT 3 =====================
    [Header("Text 3 (Absolute from Scene Start)")]
    public TMP_Text text3;
    public CanvasGroup group3;
    [TextArea] public string content3;
    public float delay3;
    public float fadeIn3 = 1f;
    public float hold3 = 2f;
    public float fadeOut3 = 1f;

    // ===================== TEXT 4 =====================
    [Header("Text 4 (Absolute from Scene Start)")]
    public TMP_Text text4;
    public CanvasGroup group4;
    [TextArea] public string content4;
    public float delay4;
    public float fadeIn4 = 1f;
    public float hold4 = 2f;
    public float fadeOut4 = 1f;

    private float sceneStartTime;

    // =================================================

    private void Awake()
    {
        sceneStartTime = Now();

        if (forceHiddenOnStart)
        {
            Hide(group1);
            Hide(group2);
            Hide(group3);
            Hide(group4);
        }
    }

    private void Start()
    {
        if (!enable) return;

        TryStart(text1, group1, content1, delay1, fadeIn1, hold1, fadeOut1);
        TryStart(text2, group2, content2, delay2, fadeIn2, hold2, fadeOut2);
        TryStart(text3, group3, content3, delay3, fadeIn3, hold3, fadeOut3);
        TryStart(text4, group4, content4, delay4, fadeIn4, hold4, fadeOut4);
    }

    // ===================== CORE =====================

    private void TryStart(
        TMP_Text text,
        CanvasGroup group,
        string content,
        float delay,
        float fadeIn,
        float hold,
        float fadeOut
    )
    {
        if (!text || !group) return;

        StartCoroutine(PlayText(text, group, content, delay, fadeIn, hold, fadeOut));
    }

    private IEnumerator PlayText(
        TMP_Text text,
        CanvasGroup group,
        string content,
        float delay,
        float fadeIn,
        float hold,
        float fadeOut
    )
    {
        group.alpha = 0f;

        yield return WaitUntilSceneTime(delay);

        text.text = content;

        yield return Fade(group, 0f, 1f, fadeIn);

        if (hold > 0f)
            yield return Wait(hold);

        yield return Fade(group, 1f, 0f, fadeOut);

        group.alpha = 0f;
    }

    // ===================== TIME =====================

    private float Now()
    {
        return useUnscaledTime ? Time.unscaledTime : Time.time;
    }

    private IEnumerator WaitUntilSceneTime(float target)
    {
        while (Now() - sceneStartTime < target)
            yield return null;
    }

    private IEnumerator Wait(float seconds)
    {
        if (useUnscaledTime)
            yield return new WaitForSecondsRealtime(seconds);
        else
            yield return new WaitForSeconds(seconds);
    }

    private IEnumerator Fade(CanvasGroup g, float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            g.alpha = to;
            yield break;
        }

        float t = 0f;
        g.alpha = from;

        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            g.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }

        g.alpha = to;
    }

    private void Hide(CanvasGroup g)
    {
        if (g) g.alpha = 0f;
    }
}
