using System.Collections;
using TMPro;
using UnityEngine;

public class Scene6IntroTwoTextController : MonoBehaviour
{
    [Header("Enable")]
    public bool enable = true;

    [Header("Options")]
    public bool useUnscaledTime = true;

    [Header("Startup Visibility")]
    public bool forceHiddenOnStart = true;

    [Header("Text 1 (Absolute from Scene Start)")]
    public TMP_Text text1;
    public CanvasGroup group1;
    [TextArea(1, 4)] public string content1 = "Text 1";
    [Min(0f)] public float delay1 = 0f;
    [Min(0f)] public float fadeIn1 = 1f;
    [Min(0f)] public float hold1 = 2f;
    [Min(0f)] public float fadeOut1 = 1f;

    [Header("Text 2 (Absolute from Scene Start)")]
    public TMP_Text text2;
    public CanvasGroup group2;
    [TextArea(1, 4)] public string content2 = "Text 2";
    [Min(0f)] public float delay2 = 0f;
    [Min(0f)] public float fadeIn2 = 1f;
    [Min(0f)] public float hold2 = 2f;
    [Min(0f)] public float fadeOut2 = 1f;

    [Header("Text 2 Fade Out Control")]
    public bool disableText2FadeOut = true; // ✅ 复选框：默认勾上 = 不自动fadeout

    private void Awake()
    {
        if (!enable) return;

        if (forceHiddenOnStart)
        {
            if (group1) group1.alpha = 0f;
            if (group2) group2.alpha = 0f;
        }

        if (text1) text1.text = content1;
        if (text2) text2.text = content2;
    }

    private void Start()
    {
        if (!enable) return;
        StartCoroutine(RunRoutine());
    }

    private IEnumerator RunRoutine()
    {
        // Text1 timeline
        if (text1 && group1)
            StartCoroutine(PlayOne(text1, group1, content1, delay1, fadeIn1, hold1, fadeOut1, true));

        // Text2 timeline (fadeOut可禁用)
        if (text2 && group2)
        {
            bool allowFadeOut = !disableText2FadeOut;
            StartCoroutine(PlayOne(text2, group2, content2, delay2, fadeIn2, hold2, fadeOut2, allowFadeOut));
        }

        yield break;
    }

    private IEnumerator PlayOne(
        TMP_Text t,
        CanvasGroup g,
        string content,
        float delay,
        float fadeIn,
        float hold,
        float fadeOut,
        bool allowFadeOut
    )
    {
        if (t) t.text = content;

        if (delay > 0f) yield return Wait(delay);

        // Fade in
        yield return Fade(g, 0f, 1f, fadeIn);

        // Hold
        if (hold > 0f) yield return Wait(hold);

        // Fade out (optional)
        if (allowFadeOut)
            yield return Fade(g, 1f, 0f, fadeOut);
        // 否则：保持在1，等待外部脚本（比如G触发）来控制淡出
    }

    private IEnumerator Wait(float seconds)
    {
        if (seconds <= 0f) yield break;
        if (useUnscaledTime) yield return new WaitForSecondsRealtime(seconds);
        else yield return new WaitForSeconds(seconds);
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
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            t += dt;
            float p = Mathf.Clamp01(t / duration);
            g.alpha = Mathf.Lerp(from, to, p);
            yield return null;
        }

        g.alpha = to;
    }
}
