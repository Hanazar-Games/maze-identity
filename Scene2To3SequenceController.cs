using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class Scene2To3SequenceController : MonoBehaviour
{
    [Header("Targets")]
    public Transform topPillar;          // 向上
    public Transform chairSet;           // 向上（你说要用 chairset 当“质点”）
    public Transform lowerPillar;        // 向上
    public Transform topWhiteBlock;      // 向下（最顶部白色区块）

    [Header("Move Offsets (World Space)")]
    public Vector3 topPillarUpOffset = new Vector3(0, 150, 0);
    public Vector3 chairSetUpOffset = new Vector3(0, 150, 0);
    public Vector3 lowerPillarUpOffset = new Vector3(0, 150, 0);
    public Vector3 topWhiteBlockDownOffset = new Vector3(0, -150, 0);

    [Header("Timings")]
    public float topPillarUpDuration = 5f;
    public float chairSetUpDuration = 5f;
    public float lowerPillarUpDuration = 5f;
    public float topWhiteBlockDownDuration = 5f;

    [Header("Flow")]
    public float delayBetweenSteps = 0.2f;

    [Header("Scene")]
    public float autoLoadScene3Delay = 8f;
    public string nextSceneName = "Scene3";

    private bool _started;

    // ========= 兼容你 Trigger 脚本里可能在调用的 BeginSequence =========
    // 1) 如果 Trigger 里是 controller.BeginSequence();
    public void BeginSequence()
    {
        StartSequence();
    }

    // 2) 如果 Trigger 里是 controller.BeginSequence(this);
    //    （你截图报错显示它在传参数，所以我也做了这个签名）
    public void BeginSequence(Scene2To3SequenceController _)
    {
        StartSequence();
    }

    // 你现在已有的入口
    public void StartSequence()
    {
        if (_started) return;
        _started = true;
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        // 记录初始位置：防止重复触发越走越远
        Vector3 topPillarStart = topPillar ? topPillar.position : Vector3.zero;
        Vector3 chairSetStart = chairSet ? chairSet.position : Vector3.zero;
        Vector3 lowerPillarStart = lowerPillar ? lowerPillar.position : Vector3.zero;
        Vector3 topWhiteBlockStart = topWhiteBlock ? topWhiteBlock.position : Vector3.zero;

        // 1) 全部同时发动（四个一起开始）
        yield return StartCoroutine(MoveMulti(
            (topPillar, topPillarStart + topPillarUpOffset, topPillarUpDuration),
            (chairSet, chairSetStart + chairSetUpOffset, chairSetUpDuration),
            (lowerPillar, lowerPillarStart + lowerPillarUpOffset, lowerPillarUpDuration),
            (topWhiteBlock, topWhiteBlockStart + topWhiteBlockDownOffset, topWhiteBlockDownDuration)
        ));

        // 2) 动画全部结束后，等一会儿切场
        yield return new WaitForSeconds(autoLoadScene3Delay);
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator MoveTo(Transform t, Vector3 targetPos, float duration)
    {
        if (t == null) yield break;

        Vector3 start = t.position;

        if (duration <= 0f)
        {
            t.position = targetPos;
            yield break;
        }

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float k = Mathf.Clamp01(time / duration);
            t.position = Vector3.Lerp(start, targetPos, k);
            yield return null;
        }
        t.position = targetPos;
    }

    private IEnumerator MoveMulti(params (Transform t, Vector3 target, float duration)[] moves)
    {
        int finished = 0;

        foreach (var m in moves)
        {
            if (m.t == null)
            {
                finished++;
                continue;
            }
            StartCoroutine(MoveTo_CountFinish(m.t, m.target, m.duration, () => finished++));
        }

        while (finished < moves.Length)
            yield return null;
    }

    private IEnumerator MoveTo_CountFinish(Transform t, Vector3 targetPos, float duration, Action onFinish)
    {
        yield return MoveTo(t, targetPos, duration);
        onFinish?.Invoke();
    }
}
