using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene4To5SequenceController : MonoBehaviour
{
    [Header("Targets")]
    public Transform mainPillar;          // Step1 主柱
    public Transform mazeChairGroup;      // Step2 迷宫椅子组
    public Light particleLight;           // Step3 灯

    [Header("Step 1: Main Pillar Up")]
    public Vector3 pillarUpOffset = new Vector3(0, 130f, 0);
    public float pillarUpTime = 6f;
    public float pillarDelay = 0f;

    [Header("Step 2: Maze Chair Group Down")]
    public Vector3 chairDownOffset = new Vector3(0, -6f, 0);
    public float chairDownTime = 4f;
    public float chairDelay = 0f;

    [Header("Step 3: Light Fade Out")]
    public float lightDelay = 2f;
    public float lightFadeTime = 4f;
    public float lightTargetIntensity = 0f;

    [Header("Step 4: Load Scene5")]
    public string scene5Name = "Scene5";
    public float loadDelay = 6.2f;

    [Header("Camera Path Movement")]
    public Camera targetCamera;                   // 不填就用 Camera.main
    public List<Transform> cameraWaypoints = new List<Transform>(); // 你自己摆点
    public float cameraDelay = 0f;                // 相机开始延迟
    public float cameraMoveDuration = 6f;         // 全程走完用时

    [Header("Camera Turning Smooth (Spline + Constant Speed)")]
    public bool useSmoothSplinePath = true;       // 勾上：转弯顺滑
    [Range(6, 80)] public int samplesPerSegment = 25; // 每段采样越高越顺（性能稍差）
    public bool clampToWaypointEndpoints = true;  // true：端点不外插（更稳）
    public bool strictConstantSpeed = true;       // true：严格恒速；false：整体缓入缓出

    [Header("Camera LookAt")]
    public bool cameraLookAtTarget = true;        // 让相机看向某目标
    public Transform cameraLookAt;                // lookAt 目标（可为空）

    [Header("Player Lock (Optional)")]
    public bool lockPlayerDuringCamera = false;   // 可选：锁玩家
    public GameObject playerRootToLock;           // 可选：玩家根物体（保留字段）
    public MonoBehaviour[] playerScriptsToDisable; // 可选：要禁用的脚本

    [Header("Mouse Look Lock")]
    public bool lockMouseLook = true;                  // 是否启用锁定
    public MonoBehaviour[] mouseLookScriptsToDisable;  // 所有控制视角的脚本
    public bool lockCursor = true;                     // 是否锁定光标

    [Header("Flow")]
    public bool oneShot = true;

    private bool _started;

    public void BeginSequence()
    {
        if (oneShot && _started) return;
        _started = true;

        // 并行启动（各自带 delay）
        StartCoroutine(MoveLocalWithDelay(mainPillar, pillarUpOffset, pillarUpTime, pillarDelay));
        StartCoroutine(MoveLocalWithDelay(mazeChairGroup, chairDownOffset, chairDownTime, chairDelay));
        StartCoroutine(FadeLight(particleLight, lightDelay, lightFadeTime, lightTargetIntensity));
        StartCoroutine(CameraPathRoutine());
        StartCoroutine(LoadSceneAfter(loadDelay, scene5Name));
    }

    private IEnumerator MoveLocalWithDelay(Transform target, Vector3 offset, float duration, float delay)
    {
        if (target == null) yield break;
        if (delay > 0f) yield return new WaitForSeconds(delay);

        if (duration <= 0f)
        {
            target.localPosition += offset;
            yield break;
        }

        Vector3 start = target.localPosition;
        Vector3 end = start + offset;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            k = SmoothStep01(k);
            target.localPosition = Vector3.Lerp(start, end, k);
            yield return null;
        }

        target.localPosition = end;
    }

    private IEnumerator FadeLight(Light l, float delay, float fadeTime, float targetIntensity)
    {
        if (l == null) yield break;
        if (delay > 0f) yield return new WaitForSeconds(delay);

        float startI = l.intensity;

        if (fadeTime <= 0f)
        {
            l.intensity = targetIntensity;
            l.enabled = (targetIntensity > 0.0001f);
            yield break;
        }

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fadeTime);
            k = SmoothStep01(k);
            l.intensity = Mathf.Lerp(startI, targetIntensity, k);
            yield return null;
        }

        l.intensity = targetIntensity;
        l.enabled = (targetIntensity > 0.0001f);
    }

    private IEnumerator CameraPathRoutine()
    {
        if (cameraWaypoints == null || cameraWaypoints.Count < 2)
            yield break;

        if (targetCamera == null)
            targetCamera = Camera.main;
        if (targetCamera == null)
            yield break;

        if (cameraDelay > 0f)
            yield return new WaitForSeconds(cameraDelay);

        if (lockMouseLook)
            SetMouseLookLock(true);

        if (lockPlayerDuringCamera)
            SetPlayerLock(true);

        // 收集路径点
        var cps = new List<Vector3>(cameraWaypoints.Count);
        for (int i = 0; i < cameraWaypoints.Count; i++)
            cps.Add(cameraWaypoints[i] ? cameraWaypoints[i].position : Vector3.zero);

        float dur = Mathf.Max(0.01f, cameraMoveDuration);

        if (!useSmoothSplinePath)
        {
            // 直线分段（原版本）——按距离恒速
            yield return MoveCameraAlongPolylineConstantSpeed(targetCamera.transform, cps, dur);
        }
        else
        {
            // 平滑曲线（Catmull-Rom）+ 按弧长恒速
            BuildSplineSamples(cps, samplesPerSegment, clampToWaypointEndpoints,
                out List<Vector3> samples, out List<float> cumDist);

            if (samples.Count >= 2)
                yield return MoveCameraAlongSamplesByDistance(targetCamera.transform, samples, cumDist, dur);
        }

        if (lockMouseLook)
            SetMouseLookLock(false);

        if (lockPlayerDuringCamera)
            SetPlayerLock(false);
    }

    // =========================
    // Movement: Polyline Constant Speed
    // =========================
    private IEnumerator MoveCameraAlongPolylineConstantSpeed(Transform cam, List<Vector3> pts, float dur)
    {
        float totalLen = 0f;
        float[] segLen = new float[pts.Count - 1];
        for (int i = 0; i < pts.Count - 1; i++)
        {
            segLen[i] = Vector3.Distance(pts[i], pts[i + 1]);
            totalLen += segLen[i];
        }
        if (totalLen <= 0.0001f) yield break;

        float elapsed = 0f;
        cam.position = pts[0];

        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            float u = Mathf.Clamp01(elapsed / dur);
            if (!strictConstantSpeed) u = SmoothStep01(u);

            Vector3 pos = EvaluatePolylineByNormalizedDistance(pts, segLen, totalLen, u);
            cam.position = pos;

            ApplyLookAtIfNeeded(cam, pos);

            yield return null;
        }

        cam.position = pts[pts.Count - 1];
        ApplyLookAtIfNeeded(cam, cam.position);
    }

    private Vector3 EvaluatePolylineByNormalizedDistance(List<Vector3> pts, float[] segLen, float totalLen, float u)
    {
        float targetDist = u * totalLen;
        float acc = 0f;

        for (int i = 0; i < segLen.Length; i++)
        {
            float next = acc + segLen[i];
            if (targetDist <= next || i == segLen.Length - 1)
            {
                float t = segLen[i] <= 0.0001f ? 0f : (targetDist - acc) / segLen[i];
                return Vector3.Lerp(pts[i], pts[i + 1], t);
            }
            acc = next;
        }
        return pts[pts.Count - 1];
    }

    // =========================
    // Movement: Spline Samples + Constant Speed
    // =========================
    private IEnumerator MoveCameraAlongSamplesByDistance(Transform cam, List<Vector3> samples, List<float> cumDist, float dur)
    {
        float totalLen = cumDist[cumDist.Count - 1];
        if (totalLen <= 0.0001f) yield break;

        float elapsed = 0f;
        cam.position = samples[0];

        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            float u = Mathf.Clamp01(elapsed / dur);
            if (!strictConstantSpeed) u = SmoothStep01(u);

            float targetDist = u * totalLen;
            Vector3 pos = EvaluateByDistance(samples, cumDist, targetDist);

            cam.position = pos;
            ApplyLookAtIfNeeded(cam, pos);

            yield return null;
        }

        cam.position = samples[samples.Count - 1];
        ApplyLookAtIfNeeded(cam, cam.position);
    }

    private void BuildSplineSamples(
        List<Vector3> controlPoints,
        int samplesPerSeg,
        bool clampEndpoints,
        out List<Vector3> samples,
        out List<float> cumDist)
    {
        samples = new List<Vector3>();
        cumDist = new List<float>();

        int n = controlPoints.Count;
        if (n < 2) return;

        Vector3 GetPoint(int idx)
        {
            if (!clampEndpoints)
            {
                idx = Mathf.Clamp(idx, 0, n - 1);
                return controlPoints[idx];
            }
            if (idx < 0) return controlPoints[0];
            if (idx >= n) return controlPoints[n - 1];
            return controlPoints[idx];
        }

        for (int i = 0; i < n - 1; i++)
        {
            Vector3 p0 = GetPoint(i - 1);
            Vector3 p1 = GetPoint(i);
            Vector3 p2 = GetPoint(i + 1);
            Vector3 p3 = GetPoint(i + 2);

            int steps = Mathf.Max(2, samplesPerSeg);
            for (int s = 0; s < steps; s++)
            {
                if (i > 0 && s == 0) continue; // 去重
                float t = (float)s / (steps - 1);
                samples.Add(CatmullRom(p0, p1, p2, p3, t));
            }
        }

        cumDist.Add(0f);
        float acc = 0f;
        for (int i = 1; i < samples.Count; i++)
        {
            acc += Vector3.Distance(samples[i - 1], samples[i]);
            cumDist.Add(acc);
        }
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    private Vector3 EvaluateByDistance(List<Vector3> samples, List<float> cumDist, float targetDist)
    {
        if (targetDist <= 0f) return samples[0];
        float totalLen = cumDist[cumDist.Count - 1];
        if (targetDist >= totalLen) return samples[samples.Count - 1];

        int lo = 0;
        int hi = cumDist.Count - 1;
        while (lo < hi)
        {
            int mid = (lo + hi) / 2;
            if (cumDist[mid] < targetDist) lo = mid + 1;
            else hi = mid;
        }

        int i1 = Mathf.Clamp(lo, 1, cumDist.Count - 1);
        int i0 = i1 - 1;

        float d0 = cumDist[i0];
        float d1 = cumDist[i1];
        float seg = Mathf.Max(0.0001f, d1 - d0);
        float t = (targetDist - d0) / seg;

        return Vector3.Lerp(samples[i0], samples[i1], t);
    }

    // =========================
    // LookAt
    // =========================
    private void ApplyLookAtIfNeeded(Transform cam, Vector3 camPos)
    {
        if (!cameraLookAtTarget) return;

        Transform look = cameraLookAt != null ? cameraLookAt : (mainPillar != null ? mainPillar : null);
        if (look == null) return;

        Vector3 dir = look.position - camPos;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
        cam.rotation = Quaternion.Slerp(cam.rotation, targetRot, 1f - Mathf.Exp(-10f * Time.deltaTime));
    }

    // =========================
    // Load Scene
    // =========================
    private IEnumerator LoadSceneAfter(float delay, string sceneName)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
    }

    // =========================
    // Locks
    // =========================
    private void SetMouseLookLock(bool locked)
    {
        if (mouseLookScriptsToDisable != null)
        {
            foreach (var s in mouseLookScriptsToDisable)
            {
                if (s != null)
                    s.enabled = !locked;
            }
        }

        if (lockCursor)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }

    private void SetPlayerLock(bool locked)
    {
        if (playerScriptsToDisable != null)
        {
            foreach (var s in playerScriptsToDisable)
                if (s != null) s.enabled = !locked;
        }
    }

    private float SmoothStep01(float x)
    {
        return x * x * (3f - 2f * x);
    }
}
