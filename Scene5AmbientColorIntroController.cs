using System.Collections;
using UnityEngine;

public class Scene5AmbientColorIntroController : MonoBehaviour
{
    [Header("Enable")]
    [Tooltip("Master switch. If off, script does nothing.")]
    public bool enable = true;

    [Header("Timing")]
    [Tooltip("Delay (seconds) after scene start before applying the ambient change.")]
    [Min(0f)] public float sceneDelay = 0f;

    [Tooltip("Animate ambient color from 'fromColor' to 'toColor' over this duration.")]
    [Min(0f)] public float duration = 1.0f;

    [Header("Ambient Color Range")]
    public Color fromColor = Color.black;
    public Color toColor = Color.gray;

    [Header("Options")]
    [Tooltip("If true, ambient color will be set to fromColor immediately on Awake (before delay).")]
    public bool setFromColorOnAwake = false;

    [Tooltip("If true, restore the original ambient color when this component is disabled.")]
    public bool restoreOnDisable = false;

    [Tooltip("If true, restore the original ambient color after the animation finishes.")]
    public bool restoreAfterFinish = false;

    [Tooltip("Use unscaled time so it works even if Time.timeScale = 0 during intro.")]
    public bool useUnscaledTime = true;

    private Color _originalAmbient;
    private Coroutine _routine;

    private void Awake()
    {
        _originalAmbient = RenderSettings.ambientLight;

        if (!enable) return;

        if (setFromColorOnAwake)
            RenderSettings.ambientLight = fromColor;
    }

    private void OnEnable()
    {
        if (!enable) return;

        // In case object gets re-enabled, refresh original value once
        _originalAmbient = RenderSettings.ambientLight;

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(PlayRoutine());
    }

    private void OnDisable()
    {
        if (restoreOnDisable)
            RenderSettings.ambientLight = _originalAmbient;
    }

    private IEnumerator PlayRoutine()
    {
        if (sceneDelay > 0f)
            yield return WaitSeconds(sceneDelay);

        // If duration is 0, snap immediately
        if (duration <= 0f)
        {
            RenderSettings.ambientLight = toColor;

            if (restoreAfterFinish)
                RenderSettings.ambientLight = _originalAmbient;

            yield break;
        }

        float t = 0f;
        // set explicit start to avoid any drift
        RenderSettings.ambientLight = fromColor;

        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            RenderSettings.ambientLight = Color.Lerp(fromColor, toColor, p);
            yield return null;
        }

        RenderSettings.ambientLight = toColor;

        if (restoreAfterFinish)
            RenderSettings.ambientLight = _originalAmbient;
    }

    private IEnumerator WaitSeconds(float seconds)
    {
        if (seconds <= 0f) yield break;

        if (useUnscaledTime)
            yield return new WaitForSecondsRealtime(seconds);
        else
            yield return new WaitForSeconds(seconds);
    }
}
