using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scene6 开场灯光演出：
/// 进场（或 Trigger 激活）→ 全黑 → 总延迟 → 质点灯淡入 → 旁边4盏灯按间隔依次淡入
/// </summary>
public class Scene6IntroLightsController : MonoBehaviour
{
    [Header("Start Mode")]
    public bool autoStartOnSceneLoad = true;   // 进场自动开始
    public bool startOnTrigger = false;        // 由 Trigger 调用 Begin() 开始（若 true 则 autoStart 不建议用）
    public float sceneStartDelay = 1f;         // 玩家进入场景后的总延迟（可调）

    [Header("Force Absolute Black At Start (Recommended)")]
    public bool forceBlackOnAwake = true;      // Awake 就强制黑（避免开场闪一帧）
    public float beforeFadeInIntensity = 0f;   // 开场强制强度（一般 0）

    [Header("Particle Light (Step 1)")]
    public Light particleLight;                 // 质点 light
    public bool particleUseDelay = true;        // 复选框：是否延迟再亮
    public float particleDelay = 0f;            // 质点延迟
    public float particleFadeInDuration = 3f;   // 质点亮起用时（可调）
    public float particleTargetIntensity = 12f; // 质点目标亮度（你也可以改成 12）

    [Header("Side Lights (0-3)")]
    public List<Light> sideLights = new List<Light>(); // 0~3 四个灯
    public float sideStartInterval = 3f;        // 每个灯开始相隔几秒（可调）
    public float sideFadeInDuration = 3f;       // 每盏灯淡入用时（可调）
    public float sideTargetIntensity = 12f;     // 旁灯目标亮度（可调）

    [Header("Target Intensity Mode")]
    public bool useOriginalIntensityAsTarget = false; // 若勾上：每盏灯淡入到它原本 inspector 的强度

    [Header("Safety")]
    public bool oneShot = true;                // 只允许播放一次
    public bool keepLightsEnabled = true;      // 亮起后保持 enabled=true（推荐 true）
    public bool logDebug = false;

    private bool _started;

    // 缓存灯原强度（用于 useOriginalIntensityAsTarget）
    private float _particleOriginalIntensity;
    private float[] _sideOriginalIntensities;

    void Awake()
    {
        CacheOriginalIntensities();

        if (forceBlackOnAwake)
            ForceAllLightsIntensity(beforeFadeInIntensity);
    }

    void Start()
    {
        if (autoStartOnSceneLoad && !startOnTrigger)
        {
            Begin();
        }
    }

    /// <summary>
    /// Trigger 调用：开始开场灯光演出
    /// </summary>
    public void Begin()
    {
        if (oneShot && _started) return;
        _started = true;

        StartCoroutine(IntroRoutine());
    }

    private void CacheOriginalIntensities()
    {
        if (particleLight != null)
            _particleOriginalIntensity = particleLight.intensity;

        _sideOriginalIntensities = new float[sideLights.Count];
        for (int i = 0; i < sideLights.Count; i++)
        {
            _sideOriginalIntensities[i] = sideLights[i] != null ? sideLights[i].intensity : 0f;
        }
    }

    private void ForceAllLightsIntensity(float intensity)
    {
        // 质点
        if (particleLight != null)
        {
            particleLight.enabled = true;
            particleLight.intensity = intensity;
        }

        // 旁灯
        for (int i = 0; i < sideLights.Count; i++)
        {
            var L = sideLights[i];
            if (L == null) continue;
            L.enabled = true;
            L.intensity = intensity;
        }
    }

    private IEnumerator IntroRoutine()
    {
        // 再兜底一次，确保 delay 期间也不亮
        if (forceBlackOnAwake)
            ForceAllLightsIntensity(beforeFadeInIntensity);

        if (sceneStartDelay > 0f)
            yield return new WaitForSeconds(sceneStartDelay);

        // Step 1: 质点灯淡入（带可选 delay）
        if (particleLight != null)
        {
            float target = useOriginalIntensityAsTarget ? _particleOriginalIntensity : particleTargetIntensity;
            StartCoroutine(FadeInLight(particleLight, beforeFadeInIntensity, target, particleFadeInDuration,
                particleUseDelay ? particleDelay : 0f));
        }

        // Step 2: 旁边 0-3 四盏灯依次淡入
        for (int i = 0; i < sideLights.Count; i++)
        {
            var L = sideLights[i];
            if (L == null) continue;

            float target = useOriginalIntensityAsTarget ? _sideOriginalIntensities[i] : sideTargetIntensity;
            float delay = i * Mathf.Max(0f, sideStartInterval);

            StartCoroutine(FadeInLight(L, beforeFadeInIntensity, target, sideFadeInDuration, delay));
        }

        if (logDebug) Debug.Log("[Scene6IntroLightsController] Intro started.", this);
    }

    private IEnumerator FadeInLight(Light L, float from, float to, float duration, float delay)
    {
        if (L == null) yield break;

        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        // 确保从 from 开始
        L.enabled = true;
        L.intensity = from;

        if (duration <= 0f)
        {
            L.intensity = to;
            if (!keepLightsEnabled && to <= 0.0001f) L.enabled = false;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            k = SmoothStep01(k);

            L.intensity = Mathf.Lerp(from, to, k);
            yield return null;
        }

        L.intensity = to;
        if (!keepLightsEnabled && to <= 0.0001f) L.enabled = false;
    }

    private float SmoothStep01(float x) => x * x * (3f - 2f * x);
}
