using System.Collections;
using UnityEngine;

public class Scene2SequenceController : MonoBehaviour
{
    [Header("Targets")]
    public Transform platform1;       // Trigger1 -> 下降的平台
    public Transform mazePlatform;    // Trigger3 -> 上升的平台
    public Transform chair;           // Trigger4 -> 下降的椅子（或椅子组父物体）
    public Light[] lightsToEnable;    // Trigger2 -> 点亮的灯（可多个）

    [Header("Platform1 Down (Step1)")]
    public Vector3 platform1DownOffset = new Vector3(0, -3f, 0);
    public float platform1MoveTime = 1.2f;

    [Header("Lights On (Step2)")]
    public bool lightsStartOff = true;     // 进入Scene2时先把灯熄灭（强度=0）
    public float lightFadeTime = 0.8f;
    public float targetIntensity = 1.2f;

    [Header("Maze Up (Step3)")]
    public Vector3 mazeUpOffset = new Vector3(0, 4f, 0);
    public float mazeMoveTime = 1.6f;

    [Header("Chair Down (Step4)")]
    public Vector3 chairDownOffset = new Vector3(0, -2.5f, 0);
    public float chairMoveTime = 1.0f;

    [Header("Flow Control")]
    public bool forceOrder = true;         // 强制 1->2->3->4 顺序
    public bool blockWhileBusy = true;     // 动画播放中不接受下一步

    // ===== 新增：自动开始（仅触发一次）=====
    [Header("Auto Start (One-shot)")]
    public bool autoStartOnSceneLoad = true;   // Scene2 进来自动开始
    public float autoStartDelay = 1.0f;        // 进入后延迟多久开始

    private int _currentStep = 0;          // 已完成到第几步（0=未开始）
    private bool _busy = false;

    private Vector3 _platform1Start;
    private Vector3 _mazeStart;
    private Vector3 _chairStart;

    private bool _autoStarted = false;     // 确保只触发一次

    private void Awake()
    {
        if (platform1) _platform1Start = platform1.position;
        if (mazePlatform) _mazeStart = mazePlatform.position;
        if (chair) _chairStart = chair.position;

        // 可选：进入 Scene2 默认灯先“灭”（强度=0，但灯组件保持 enabled）
        if (lightsToEnable != null)
        {
            foreach (var l in lightsToEnable)
            {
                if (!l) continue;
                l.enabled = true;
                if (lightsStartOff) l.intensity = 0f;
            }
        }
    }

    // ===== 新增：进入场景自动开始 =====
    private void Start()
    {
        if (autoStartOnSceneLoad && !_autoStarted)
        {
            StartCoroutine(AutoStartRoutine());
        }
    }

    private IEnumerator AutoStartRoutine()
    {
        _autoStarted = true;

        if (autoStartDelay > 0f)
            yield return new WaitForSeconds(autoStartDelay);

        // 按 1->2->3->4 自动走一次（等待每一步完成）
        OnStepTriggered(1);
        yield return null;
        while (_busy) yield return null;

        OnStepTriggered(2);
        yield return null;
        while (_busy) yield return null;

        OnStepTriggered(3);
        yield return null;
        while (_busy) yield return null;

        OnStepTriggered(4);
        yield return null;
        while (_busy) yield return null;
    }

    /// <summary>
    /// 由每个 Trigger 调用：stepIndex = 1,2,3,4
    /// </summary>
    public void OnStepTriggered(int stepIndex)
    {
        if (blockWhileBusy && _busy) return;

        // 强制顺序：只允许触发“下一步”
        if (forceOrder && stepIndex != _currentStep + 1) return;

        // 防止重复触发同一步（比如玩家来回踩）
        if (stepIndex <= _currentStep) return;

        _currentStep = stepIndex;

        switch (stepIndex)
        {
            case 1:
                StartCoroutine(Step1_PlatformDown());
                break;
            case 2:
                StartCoroutine(Step2_LightsOn());
                break;
            case 3:
                StartCoroutine(Step3_MazeUp());
                break;
            case 4:
                StartCoroutine(Step4_ChairDown());
                break;
        }
    }

    private IEnumerator Step1_PlatformDown()
    {
        _busy = true;

        if (platform1)
        {
            Vector3 from = _platform1Start;
            Vector3 to = _platform1Start + platform1DownOffset;
            yield return MoveWorld(platform1, from, to, platform1MoveTime);
        }

        _busy = false;
    }

    private IEnumerator Step2_LightsOn()
    {
        _busy = true;

        if (lightsToEnable != null && lightsToEnable.Length > 0)
        {
            // 以当前强度作为起点（更稳）
            float fromIntensity = 0f;
            for (int i = 0; i < lightsToEnable.Length; i++)
            {
                if (lightsToEnable[i]) { fromIntensity = lightsToEnable[i].intensity; break; }
            }

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / Mathf.Max(0.01f, lightFadeTime);
                float v = Mathf.Lerp(fromIntensity, targetIntensity, EaseOut(t));
                foreach (var l in lightsToEnable)
                {
                    if (!l) continue;
                    l.enabled = true;
                    l.intensity = v;
                }
                yield return null;
            }

            foreach (var l in lightsToEnable)
            {
                if (!l) continue;
                l.enabled = true;
                l.intensity = targetIntensity;
            }
        }

        _busy = false;
    }

    private IEnumerator Step3_MazeUp()
    {
        _busy = true;

        if (mazePlatform)
        {
            Vector3 from = _mazeStart;
            Vector3 to = _mazeStart + mazeUpOffset;
            yield return MoveWorld(mazePlatform, from, to, mazeMoveTime);
        }

        _busy = false;
    }

    private IEnumerator Step4_ChairDown()
    {
        _busy = true;

        if (chair)
        {
            Vector3 from = _chairStart;
            Vector3 to = _chairStart + chairDownOffset;
            yield return MoveWorld(chair, from, to, chairMoveTime);
        }

        _busy = false;
    }

    private IEnumerator MoveWorld(Transform tr, Vector3 from, Vector3 to, float time)
    {
        if (!tr) yield break;

        if (time <= 0.01f)
        {
            tr.position = to;
            yield break;
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, time);
            tr.position = Vector3.Lerp(from, to, EaseInOut(t));
            yield return null;
        }

        tr.position = to;
    }

    private float EaseInOut(float t) => t * t * (3f - 2f * t);
    private float EaseOut(float t) => 1f - (1f - t) * (1f - t);

    // ===== 可选：如果你想重置整个流程（比如重开关卡）=====
    public void ResetSequence(bool resetTransforms = true)
    {
        StopAllCoroutines();
        _busy = false;
        _currentStep = 0;
        _autoStarted = false;

        if (resetTransforms)
        {
            if (platform1) platform1.position = _platform1Start;
            if (mazePlatform) mazePlatform.position = _mazeStart;
            if (chair) chair.position = _chairStart;

            if (lightsToEnable != null)
            {
                foreach (var l in lightsToEnable)
                {
                    if (!l) continue;
                    l.enabled = true;
                    if (lightsStartOff) l.intensity = 0f;
                }
            }
        }
    }
}
