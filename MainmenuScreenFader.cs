using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    [Header("Overlay (UI Image)")]
    public UnityEngine.UI.Image fadeOverlay;   // Canvas/FadeOverlay
    public bool blockInputWhileFading = true;

    [Header("Fade In (on scene enter)")]
    public bool enableFadeIn = true;
    [Min(0f)] public float fadeInDuration = 0.8f;
    [Min(0f)] public float fadeInHoldBlack = 0.05f; // 进入先黑一小段，可遮首帧卡顿

    [Header("Fade Out (before action)")]
    public bool enableFadeOut = true;
    [Min(0f)] public float fadeOutDuration = 0.6f;
    [Min(0f)] public float fadeOutHoldBlack = 0.0f; // FadeOut结束后额外黑屏停留

    private CanvasGroup _cg;
    private Coroutine _co;
    private bool _busy;

    private void Awake()
    {
        if (fadeOverlay == null)
        {
            Debug.LogWarning("[ScreenFader] fadeOverlay 未绑定。请把 Canvas/FadeOverlay(Image) 拖进来。");
            return;
        }

        _cg = fadeOverlay.GetComponent<CanvasGroup>();
        if (_cg == null) _cg = fadeOverlay.gameObject.AddComponent<CanvasGroup>();

        // 进入场景时强制先黑，避免闪烁
        SetOverlayAlpha(1f);
        SetOverlayBlock(blockInputWhileFading);
    }

    private void Start()
    {
        if (fadeOverlay == null) return;

        if (enableFadeIn)
        {
            StartFadeIn();
        }
        else
        {
            SetOverlayAlpha(0f);
            SetOverlayBlock(false);
        }
    }

    public void StartFadeIn()
    {
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(FadeInRoutine());
    }

    public void FadeOutThen(System.Action afterFadeOut)
    {
        if (fadeOverlay == null)
        {
            afterFadeOut?.Invoke();
            return;
        }

        if (!enableFadeOut)
        {
            afterFadeOut?.Invoke();
            return;
        }

        if (_busy) return;
        _busy = true;

        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(FadeOutRoutine(afterFadeOut));
    }

    private IEnumerator FadeInRoutine()
    {
        _busy = true;

        SetOverlayAlpha(1f);
        SetOverlayBlock(blockInputWhileFading);

        if (fadeInHoldBlack > 0f)
            yield return new WaitForSeconds(fadeInHoldBlack);

        float dur = Mathf.Max(0.01f, fadeInDuration);
        float t = 0f;

        while (t < dur)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / dur);
            k = SmoothStep01(k);

            SetOverlayAlpha(1f - k);
            yield return null;
        }

        SetOverlayAlpha(0f);
        SetOverlayBlock(false);

        _busy = false;
        _co = null;
    }

    private IEnumerator FadeOutRoutine(System.Action afterFadeOut)
    {
        SetOverlayBlock(blockInputWhileFading);

        float dur = Mathf.Max(0.01f, fadeOutDuration);
        float t = 0f;

        // 从当前 alpha 开始（兼容你手动设置）
        float from = _cg != null ? _cg.alpha : fadeOverlay.color.a;

        while (t < dur)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / dur);
            k = SmoothStep01(k);

            SetOverlayAlpha(Mathf.Lerp(from, 1f, k));
            yield return null;
        }

        SetOverlayAlpha(1f);

        if (fadeOutHoldBlack > 0f)
            yield return new WaitForSeconds(fadeOutHoldBlack);

        afterFadeOut?.Invoke();

        // 注意：这里不把 busy=false，因为通常 afterFadeOut 会切场景
        _co = null;
    }

    private void SetOverlayAlpha(float a)
    {
        a = Mathf.Clamp01(a);

        if (_cg != null) _cg.alpha = a;

        var c = fadeOverlay.color;
        c.a = a;
        fadeOverlay.color = c;
    }

    private void SetOverlayBlock(bool block)
    {
        if (_cg == null) return;
        _cg.blocksRaycasts = block;
        _cg.interactable = block;
    }

    private float SmoothStep01(float x) => x * x * (3f - 2f * x);
}
