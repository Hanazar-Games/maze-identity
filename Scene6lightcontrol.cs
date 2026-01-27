using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBlinkController : MonoBehaviour
{
    [Header("Target Lights (Multi)")]
    public Light[] targetLights; // ∂‡µ∆≤€£®≥°æ∞µ∆ + Õ∑µ∆£©

    [Header("Legacy (Single Light)")]
    public Light targetLight;    // ºÊ»›æ…≤€£®ø…≤ªÃÓ£©

    [Header("Initial State")]
    public float initialIntensity = 0f;
    public bool applyInitialOnAwake = true;

    [Header("Activation")]
    public bool oneShot = true;
    public float activateDelay = 0f;

    [Header("Blink Intensity")]
    public float onHoldIntensity = 12f;

    [Header("Blink Timing")]
    public float fadeUpDuration = 0.5f;
    public float onHoldDuration = 0.3f;
    public float fadeDownDuration = 0.5f;
    public float offHoldDuration = 0.3f;

    [Header("Curve")]
    public bool useSmoothStep = true;

    [Header("Runtime Control")]
    public bool startActive = false;
    public bool stopDisablesLight = false;

    private bool _activated;
    private Coroutine _loopCo;

    private enum Phase { Idle, OffHold, FadingUp, OnHold, FadingDown }
    private Phase _phase = Phase.Idle;
    private float _currentIntensity = 0f;

    private bool _requestStopAtBright = false;
    private bool _disableControllerWhenStopped = true;
    private bool _leaveLightEnabledWhenStopped = true;

    void Awake()
    {
        if (applyInitialOnAwake)
            ForceIntensity(initialIntensity);
    }

    void Start()
    {
        if (startActive)
            Activate();
    }

    public void Activate()
    {
        if (oneShot && _activated) return;
        _activated = true;

        _requestStopAtBright = false;

        if (_loopCo != null)
            StopCoroutine(_loopCo);

        _loopCo = StartCoroutine(BlinkLoop());
    }

    public void StopBlink()
    {
        _requestStopAtBright = false;

        if (_loopCo != null)
        {
            StopCoroutine(_loopCo);
            _loopCo = null;
        }

        var lights = GetAllLights();
        if (lights.Count == 0) return;

        if (stopDisablesLight)
        {
            SetAllIntensity(lights, 0f);
            SetAllEnabled(lights, false);
            _phase = Phase.Idle;
            _currentIntensity = 0f;
        }
        else
        {
            ForceIntensity(initialIntensity);
            _phase = Phase.Idle;
            _currentIntensity = initialIntensity;
        }
    }

    public void StopAtBrightThenDisable(bool disableThisController = true, bool keepLightEnabledAtBright = true)
    {
        _requestStopAtBright = true;
        _disableControllerWhenStopped = disableThisController;
        _leaveLightEnabledWhenStopped = keepLightEnabledAtBright;

        if (_loopCo == null)
            HoldBrightAndDisableNow();
    }

    // ---------- NEW: dynamic add/remove ----------
    public void AddTargetLight(Light L)
    {
        if (L == null) return;

        if (targetLights == null)
        {
            targetLights = new Light[] { L };
            return;
        }

        for (int i = 0; i < targetLights.Length; i++)
            if (targetLights[i] == L) return;

        var arr = new Light[targetLights.Length + 1];
        for (int i = 0; i < targetLights.Length; i++) arr[i] = targetLights[i];
        arr[arr.Length - 1] = L;
        targetLights = arr;
    }

    public void RemoveTargetLight(Light L)
    {
        if (L == null || targetLights == null || targetLights.Length == 0) return;

        int count = 0;
        for (int i = 0; i < targetLights.Length; i++)
            if (targetLights[i] != null && targetLights[i] != L) count++;

        var arr = new Light[count];
        int idx = 0;
        for (int i = 0; i < targetLights.Length; i++)
        {
            var t = targetLights[i];
            if (t != null && t != L) arr[idx++] = t;
        }
        targetLights = arr;
    }

    // ---------- loop ----------
    private IEnumerator BlinkLoop()
    {
        var lights = GetAllLights();
        if (lights.Count == 0) yield break;

        if (activateDelay > 0f)
            yield return new WaitForSeconds(activateDelay);

        ForceIntensity(initialIntensity);
        _currentIntensity = initialIntensity;

        while (true)
        {
            _phase = Phase.OffHold;
            if (offHoldDuration > 0f) yield return new WaitForSeconds(offHoldDuration);

            _phase = Phase.FadingUp;
            yield return FadeTo(onHoldIntensity, fadeUpDuration);
            if (_phase == Phase.Idle) yield break;

            _phase = Phase.OnHold;

            if (_requestStopAtBright)
            {
                HoldBrightAndDisableNow();
                yield break;
            }

            if (onHoldDuration > 0f) yield return new WaitForSeconds(onHoldDuration);

            _phase = Phase.FadingDown;
            yield return FadeTo(0f, fadeDownDuration);
            if (_phase == Phase.Idle) yield break;
        }
    }

    private IEnumerator FadeTo(float target, float duration)
    {
        var lights = GetAllLights();
        if (lights.Count == 0) yield break;

        SetAllEnabled(lights, true);

        float from = _currentIntensity;

        if (duration <= 0f)
        {
            ForceIntensity(target);
            _currentIntensity = target;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            if (_requestStopAtBright && _phase == Phase.FadingDown)
            {
                yield return FadeDirect(_currentIntensity, onHoldIntensity, Mathf.Max(0.01f, fadeUpDuration));
                HoldBrightAndDisableNow();
                yield break;
            }

            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            if (useSmoothStep) k = SmoothStep01(k);

            float val = Mathf.Lerp(from, target, k);
            ForceIntensity(val);
            _currentIntensity = val;

            yield return null;
        }

        ForceIntensity(target);
        _currentIntensity = target;
    }

    private IEnumerator FadeDirect(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            ForceIntensity(to);
            _currentIntensity = to;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            if (useSmoothStep) k = SmoothStep01(k);

            float val = Mathf.Lerp(from, to, k);
            ForceIntensity(val);
            _currentIntensity = val;

            yield return null;
        }

        ForceIntensity(to);
        _currentIntensity = to;
    }

    private void HoldBrightAndDisableNow()
    {
        var lights = GetAllLights();
        if (lights.Count == 0)
        {
            _phase = Phase.Idle;
            return;
        }

        SetAllEnabled(lights, true);
        SetAllIntensity(lights, onHoldIntensity);
        _currentIntensity = onHoldIntensity;

        if (_loopCo != null)
        {
            StopCoroutine(_loopCo);
            _loopCo = null;
        }

        _phase = Phase.Idle;
        _requestStopAtBright = false;

        if (!_leaveLightEnabledWhenStopped)
        {
            SetAllIntensity(lights, 0f);
            SetAllEnabled(lights, false);
            _currentIntensity = 0f;
        }

        if (_disableControllerWhenStopped)
            this.enabled = false;
    }

    private void ForceIntensity(float value)
    {
        var lights = GetAllLights();
        if (lights.Count == 0) return;

        SetAllEnabled(lights, true);
        SetAllIntensity(lights, value);
    }

    private List<Light> GetAllLights()
    {
        var list = new List<Light>(8);

        if (targetLights != null)
        {
            for (int i = 0; i < targetLights.Length; i++)
                if (targetLights[i] != null) list.Add(targetLights[i]);
        }

        if (targetLight != null && !list.Contains(targetLight))
            list.Add(targetLight);

        return list;
    }

    private void SetAllEnabled(List<Light> lights, bool enabled)
    {
        for (int i = 0; i < lights.Count; i++)
            if (lights[i] != null) lights[i].enabled = enabled;
    }

    private void SetAllIntensity(List<Light> lights, float intensity)
    {
        for (int i = 0; i < lights.Count; i++)
            if (lights[i] != null) lights[i].intensity = intensity;
    }

    private float SmoothStep01(float x) => x * x * (3f - 2f * x);
}
