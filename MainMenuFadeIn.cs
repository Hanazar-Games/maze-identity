using System.Collections;
using UnityEngine;
using UnityEngine.UI; // 保留
//  确保【没有】 using System.Net.Mime;

public class MainMenuFadeIn : MonoBehaviour
{
    [Header("Enable")]
    public bool enableFadeIn = true;

    [Header("Timing")]
    [Min(0f)] public float fadeInDuration = 0.8f;
    [Min(0f)] public float holdBlackTime = 0.0f;

    [Header("Overlay (UI Image)")]
    public UnityEngine.UI.Image fadeOverlay; // ✅ 显式指定
    public bool blockInputWhileFading = true;

    private CanvasGroup _cg;
    private Coroutine _co;

    private void Awake()
    {
        if (fadeOverlay == null)
        {
            Debug.LogWarning("[MainMenuFadeIn] fadeOverlay 未绑定。");
            return;
        }

        _cg = fadeOverlay.GetComponent<CanvasGroup>();
        if (_cg == null)
            _cg = fadeOverlay.gameObject.AddComponent<CanvasGroup>();

        SetOverlayAlpha(1f);
        SetOverlayRaycast(blockInputWhileFading);
    }

    private void Start()
    {
        if (!enableFadeIn)
        {
            SetOverlayAlpha(0f);
            SetOverlayRaycast(false);
            return;
        }

        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        if (holdBlackTime > 0f)
            yield return new WaitForSeconds(holdBlackTime);

        float dur = Mathf.Max(0.01f, fadeInDuration);
        float t = 0f;

        while (t < dur)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / dur);
            k = k * k * (3f - 2f * k); // SmoothStep

            SetOverlayAlpha(1f - k);
            yield return null;
        }

        SetOverlayAlpha(0f);
        SetOverlayRaycast(false);
        _co = null;
    }

    private void SetOverlayAlpha(float a)
    {
        if (_cg != null) _cg.alpha = a;

        var c = fadeOverlay.color;
        c.a = Mathf.Clamp01(a);
        fadeOverlay.color = c;
    }

    private void SetOverlayRaycast(bool block)
    {
        if (_cg == null) return;
        _cg.blocksRaycasts = block;
        _cg.interactable = block;
    }
}
