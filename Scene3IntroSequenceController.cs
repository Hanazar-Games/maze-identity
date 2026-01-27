using System.Collections;
using UnityEngine;

public class Scene3IntroSequenceController : MonoBehaviour
{
    [Header("Trigger / One-shot")]
    public bool playOnce = true;
    private bool _played = false;

    [Header("Flow Control")]
    public bool forceOrder = true;       // 勾选=强制顺序；不勾=允许外部乱序触发
    public bool blockWhileBusy = true;   // 动画播放中不接受下一步

    private int _currentStep = 0;
    private bool _busy = false;

    [Header("Player")]
    public PlayerMove player;

    [Header("Player Override Values (Step2)")]
    public float gravityValue = 5f;
    public float walkSpeedValue = 4f;
    public float runSpeedValue = 30f;
    public float jumpHeightValue = 35f;      // 对应 PlayerMove.jumpHeight
    public float terminalVelocityValue = 20f;

    [Header("Fog (Step1)")]
    public bool enableFogIfOff = true;
    public float fogStartDensity = 0.1f;
    public float fogMidDensity = 0f;
    public float fogEndDensity = 0.007f;

    public float fogToZeroTime = 1.2f;       // 0.1 -> 0 用时
    public float fogHoldAtZeroTime = 0.8f;   // ✅ 新增：在 0 停留多久
    public float fogToEndTime = 1.0f;        // 0 -> 0.007 用时

    // 用于确保 StartSequence 不会被重复启动
    private Coroutine _sequenceRoutine;

    /// <summary>
    /// 入口：玩家进入 trigger 后调用，只触发一次
    /// </summary>
    public void StartSequence()
    {
        if (playOnce && _played) return;
        _played = true;

        if (_sequenceRoutine != null) return;
        _sequenceRoutine = StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        // 1) 先跑 Fog（必须完整结束）
        yield return StartCoroutine(Step1_Fog());

        // 2) Fog 完整结束后，才改玩家参数
        yield return StartCoroutine(Step2_PlayerOverride());

        _sequenceRoutine = null;
    }

    /// <summary>
    /// 如果你仍想支持外部“分步触发”（可选），保留这个接口
    /// </summary>
    public void OnStepTriggered(int stepIndex)
    {
        if (blockWhileBusy && _busy) return;
        if (forceOrder && stepIndex != _currentStep + 1) return;
        if (stepIndex <= _currentStep) return;

        _currentStep = stepIndex;

        switch (stepIndex)
        {
            case 1:
                StartCoroutine(Step1_Fog());
                break;
            case 2:
                StartCoroutine(Step2_PlayerOverride());
                break;
        }
    }

    private IEnumerator Step1_Fog()
    {
        _busy = true;

        if (enableFogIfOff && !RenderSettings.fog)
            RenderSettings.fog = true;

        // 0.1 -> 0
        yield return LerpFogDensity(fogStartDensity, fogMidDensity, fogToZeroTime);

        // ✅ 在 0 停留
        if (fogHoldAtZeroTime > 0f)
            yield return new WaitForSeconds(fogHoldAtZeroTime);

        // 0 -> 0.007
        yield return LerpFogDensity(fogMidDensity, fogEndDensity, fogToEndTime);

        _busy = false;
    }

    private IEnumerator Step2_PlayerOverride()
    {
        _busy = true;

        if (player != null)
        {
            player.gravity = gravityValue;
            player.walkSpeed = walkSpeedValue;
            player.runSpeed = runSpeedValue;
            player.jumpHeight = jumpHeightValue;
            player.terminalVelocity = terminalVelocityValue;
        }
        else
        {
            Debug.LogWarning("[Scene3IntroSequenceController] Player is NULL");
        }

        _busy = false;
        yield break;
    }

    private IEnumerator LerpFogDensity(float from, float to, float time)
    {
        if (time <= 0.01f)
        {
            RenderSettings.fogDensity = to;
            yield break;
        }

        RenderSettings.fogDensity = from;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, time);
            RenderSettings.fogDensity = Mathf.Lerp(from, to, EaseInOut(t));
            yield return null;
        }

        RenderSettings.fogDensity = to;
    }

    private float EaseInOut(float t) => t * t * (3f - 2f * t);

    public void ResetSequence()
    {
        if (_sequenceRoutine != null)
        {
            StopCoroutine(_sequenceRoutine);
            _sequenceRoutine = null;
        }

        StopAllCoroutines();
        _busy = false;
        _currentStep = 0;
        _played = false;
    }
}
