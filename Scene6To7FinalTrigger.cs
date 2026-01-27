using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene6 终章 Trigger：
/// 玩家进入 → 灯光平滑灭 → 延迟 → 跳转 Scene7
/// </summary>
public class Scene6To7FinalTrigger : MonoBehaviour
{
    [Header("Trigger")]
    public bool triggerOnce = true;
    private bool _triggered;

    [Header("Lights To Turn Off")]
    public Light[] lightsToFadeOut;

    [Header("Light Fade")]
    public float lightFadeDuration = 1.0f;   // 灯灭用时
    public float lightFadeDelay = 0f;         // 触发后多久开始灭
    public float targetIntensity = 0f;        // 一般为 0

    [Header("Scene Transition")]
    public string scene7Name = "Scene7";
    public float loadSceneDelay = 1.2f;       // 灯灭后多久切场

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered && triggerOnce) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;

        StartCoroutine(FinalSequence());
    }

    private IEnumerator FinalSequence()
    {
        if (lightFadeDelay > 0f)
            yield return new WaitForSeconds(lightFadeDelay);

        // 灯灭（并行）
        if (lightsToFadeOut != null && lightsToFadeOut.Length > 0)
            yield return StartCoroutine(FadeOutLights());

        // 延迟后切场
        if (loadSceneDelay > 0f)
            yield return new WaitForSeconds(loadSceneDelay);

        if (!string.IsNullOrEmpty(scene7Name))
            SceneManager.LoadScene(scene7Name);
    }

    private IEnumerator FadeOutLights()
    {
        float dur = Mathf.Max(0.01f, lightFadeDuration);

        // 记录初始亮度
        float[] startInt = new float[lightsToFadeOut.Length];
        for (int i = 0; i < lightsToFadeOut.Length; i++)
        {
            var L = lightsToFadeOut[i];
            if (L == null)
            {
                startInt[i] = 0f;
                continue;
            }

            L.enabled = true; // 确保可控
            startInt[i] = L.intensity;
        }

        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / dur);
            k = SmoothStep01(k);

            for (int i = 0; i < lightsToFadeOut.Length; i++)
            {
                var L = lightsToFadeOut[i];
                if (L == null) continue;

                L.intensity = Mathf.Lerp(startInt[i], targetIntensity, k);
            }

            yield return null;
        }

        // 收尾
        for (int i = 0; i < lightsToFadeOut.Length; i++)
        {
            var L = lightsToFadeOut[i];
            if (L == null) continue;

            L.intensity = targetIntensity;
            L.enabled = false; // 彻底灭
        }
    }

    private float SmoothStep01(float x)
    {
        return x * x * (3f - 2f * x);
    }
}
