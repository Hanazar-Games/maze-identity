using System.Collections;
using UnityEngine;

public class FogPulseHelper : MonoBehaviour
{
    [Header("Schedule")]
    public bool enablePulse = true;
    public float intervalSeconds = 60f;     // 每分钟一次
    public float initialDelay = 30f;        // 进场后多久开始第一次（可改 0 / 10 / 30）

    [Header("Fog Densities")]
    public float normalDensity = 0.007f;    // 你的常态雾
    public float clearDensity = 0.001f;     // “消散后”的雾密度（不是0，更自然）

    [Header("Timings")]
    public float fadeOutTime = 0.7f;        // normal -> clear
    public float holdClearTime = 2.0f;      // 清晰窗口停留
    public float fadeInTime = 1.2f;         // clear -> normal

    [Header("Optional: Hint Object")]
    public GameObject hintObject;           // 可选：指引箭头/灯/粒子
    public float hintOnTime = 2.0f;

    private Coroutine _loop;

    void OnEnable()
    {
        if (_loop == null) _loop = StartCoroutine(Loop());
    }

    void OnDisable()
    {
        if (_loop != null) StopCoroutine(_loop);
        _loop = null;
    }

    private IEnumerator Loop()
    {
        if (initialDelay > 0f) yield return new WaitForSeconds(initialDelay);

        while (enablePulse)
        {
            yield return StartCoroutine(PulseOnce());
            yield return new WaitForSeconds(Mathf.Max(1f, intervalSeconds));
        }
    }

    public IEnumerator PulseOnce()
    {
        // 确保雾开着
        if (!RenderSettings.fog) RenderSettings.fog = true;

        // 可选提示物体闪一下
        if (hintObject != null)
        {
            hintObject.SetActive(true);
            StartCoroutine(HintOffLater());
        }

        // normal -> clear
        yield return LerpFog(RenderSettings.fogDensity, clearDensity, fadeOutTime);

        // hold
        if (holdClearTime > 0f) yield return new WaitForSeconds(holdClearTime);

        // clear -> normal
        yield return LerpFog(RenderSettings.fogDensity, normalDensity, fadeInTime);
    }

    private IEnumerator HintOffLater()
    {
        yield return new WaitForSeconds(Mathf.Max(0.1f, hintOnTime));
        if (hintObject != null) hintObject.SetActive(false);
    }

    private IEnumerator LerpFog(float from, float to, float time)
    {
        if (time <= 0.01f)
        {
            RenderSettings.fogDensity = to;
            yield break;
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / time;
            RenderSettings.fogDensity = Mathf.Lerp(from, to, EaseInOut(t));
            yield return null;
        }
        RenderSettings.fogDensity = to;
    }

    private float EaseInOut(float t) => t * t * (3f - 2f * t);
}
