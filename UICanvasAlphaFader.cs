using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 用于测试/驱动 UI Image 的 alpha（不依赖 CanvasGroup）
/// 3) UICanvasAlphaFader.cs（可选：你想单独验证 FadeOverlay 是否能工作时用）
/// </summary>
public class UICanvasAlphaFader : MonoBehaviour
{
    public Image img;
    public KeyCode testKey = KeyCode.F9;

    [Min(0.01f)] public float duration = 0.5f;

    private Coroutine _co;

    private void Awake()
    {
        if (img == null) img = GetComponent<Image>();
        if (img != null)
        {
            // 确保在最顶层
            img.transform.SetAsLastSibling();
        }
    }

    private void Update()
    {
        if (img == null) return;

        if (Input.GetKeyDown(testKey))
        {
            // 透明 -> 黑
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(FadeTo(1f));
        }
    }

    public void FadeInToClear()
    {
        if (img == null) return;
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(FadeTo(0f));
    }

    public void FadeOutToBlack()
    {
        if (img == null) return;
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(FadeTo(1f));
    }

    private IEnumerator FadeTo(float target)
    {
        img.transform.SetAsLastSibling();

        Color c = img.color;
        float start = c.a;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            c.a = Mathf.Lerp(start, target, k);
            img.color = c;
            yield return null;
        }

        c.a = target;
        img.color = c;
        _co = null;
    }
}
