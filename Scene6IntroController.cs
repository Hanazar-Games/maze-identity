using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene6IntroController : MonoBehaviour
{
    [Header("Auto Start")]
    public bool autoStartOnSceneLoad = true;
    public float autoStartDelay = 1f;

    [Header("Camera")]
    public Camera targetCamera;

    [Header("Camera Path A (Ceiling Loop)")]
    public List<Transform> pathA = new List<Transform>();
    public float pathADelay = 0f;
    public float pathADuration = 6f;
    public bool startUpsideDown = true;
    public float upsideDownRollDegrees = 180f;

    [Header("A -> B Transition (Smooth)")]
    public bool useTransitionAtoB = true;
    public float transitionExtraDelay = 0f;
    public float transitionDuration = 1.0f;
    public float transitionRollDegrees = 180f;

    [Header("Camera Path B (After Transition)")]
    public List<Transform> pathB = new List<Transform>();
    public float pathBDelay = 0f;
    public float pathBDuration = 5f;

    [Header("Return To Player (Smooth)")]
    public bool returnToPlayerAfterCinematic = true;
    public Transform playerCameraPivot;            // Player/CameraPivot
    public Transform playerMainCameraTransform;    // Player/CameraPivot/Main Camera
    public float returnDelay = 0f;
    public float returnDuration = 0.8f;
    public bool reparentToPivotOnFinish = true;

    [Header("Path Smoothing (Spline + Constant Speed)")]
    public bool useSmoothSplinePath = true;
    [Range(6, 80)] public int samplesPerSegment = 25;
    public bool clampToWaypointEndpoints = true;
    public bool strictConstantSpeed = true; // true=严格恒速；false=缓入缓出

    [Header("LookAt Mode (Single Target)")]
    public bool alwaysLookAtSingleTarget = true;
    public Transform alwaysLookAtTarget;
    public float lookAtSmooth = 10f;
    [Range(0f, 1f)] public float lookAtStrength = 1f;
    public bool keepRollWhileLooking = false;
    public bool lookAtDuringTransition = true;
    public bool lookAtDuringReturn = false;

    [Header("Chair (Maze Top Chair Group)")]
    public Transform mazeTopChairGroup;
    public float chairDelay = 4f;
    public Vector3 chairUpOffset = new Vector3(0, 6f, 0);
    public float chairMoveTime = 3f;

    [Header("Maze Massive Raise (Unified Delay)")]
    public float mazeUnifiedDelay = 12f;

    [Serializable]
    public class MoveItem
    {
        public string name;
        public Transform target;
        public Vector3 localOffset;
        public float moveTime;
        public float extraDelay;
    }

    [Header("Maze Move Items (All start after mazeUnifiedDelay)")]
    public List<MoveItem> mazeMoves = new List<MoveItem>();

    [Header("Input Lock (Optional)")]
    public bool lockMouseLook = true;
    public MonoBehaviour[] mouseLookScriptsToDisable;
    public bool lockCursor = true;

    [Header("Roll Safety")]
    public bool forceCameraRollZAtStart = false;
    public float forcedStartRollZ = 0f;

    // =========================
    // Lights: Start Dark -> Fade In -> (optional) Fade Out at End
    // =========================

    [Header("Lights Fade-In Safety")]
    public float beforeFadeInLightIntensity = 0f; // 兜底：FadeIn 前绝对强制的亮度（0=全黑）

    [Header("Lights - Start Dark")]
    public bool forceLightsDarkOnAwake = true;   // 开场就黑（直接改 intensity=0）
    public Light[] controlledLights;             // 统一控制的灯（淡入/淡出都用它）

    [Header("Lights Fade In (Start Dark -> Bright)")]
    public bool fadeInLightsAtStart = true;
    public float lightsFadeInDelay = 0f;
    public float lightsFadeInDuration = 2f;
    public float lightsFadeInTargetIntensity = 20f; // 亮到多少（统一目标）

    [Header("Lights Fade Out (End)")]
    public bool fadeOutLights = true;
    public float lightsFadeOutDelay = 0f;
    public float lightsFadeOutDuration = 2f;
    public float lightsFadeOutTargetIntensity = 0f;
    public bool lightsFadeOutDelayFromCinematicStart = false; // false=演出结束后算delay
    public bool disableLightsAfterFadeOut = true;

    [Header("Debug")]
    public bool logWhenBegin = false;
    public bool logLights = false;

    bool _started;
    float _cinematicStartTime;

    // 玩家相机缓存
    Transform _cachedPlayerPivot;
    Transform _cachedPlayerCam;
    Vector3 _cachedPlayerCamLocalPos;
    Quaternion _cachedPlayerCamLocalRot;

    // 灯强度缓存（用于开场强制黑后，再淡入时从0到目标）
    float[] _cachedOriginalIntensities;

    void Awake()
    {
        ForceLightsToBeforeFadeInState();
    }


    void ForceLightsToBeforeFadeInState()
    {
        if (controlledLights == null) return;

        for (int i = 0; i < controlledLights.Length; i++)
        {
            var L = controlledLights[i];
            if (L == null) continue;

            L.enabled = true; // 必须启用，否则 intensity 不生效
            L.intensity = beforeFadeInLightIntensity;
        }
    }



    void Start()
    {
        if (autoStartOnSceneLoad)
            StartCoroutine(AutoStartRoutine());
    }

    IEnumerator AutoStartRoutine()
    {
        if (autoStartDelay > 0f)
            yield return new WaitForSeconds(autoStartDelay);

        BeginIntro();
    }

    public void BeginIntro()
    {
        if (_started) return;
        _started = true;

        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera == null) return;

        CachePlayerCameraPose();
        CacheLightOriginalsIfNeeded();

        if (forceCameraRollZAtStart)
        {
            var e = targetCamera.transform.eulerAngles;
            e.z = forcedStartRollZ;
            targetCamera.transform.eulerAngles = e;
        }

        if (logWhenBegin)
            Debug.Log("[Scene6IntroController] BeginIntro()", this);

        _cinematicStartTime = Time.time;

        if (lockMouseLook) SetMouseLookLock(true);

        // 灯：开场淡入（从0到目标）
        if (fadeInLightsAtStart)
            StartCoroutine(FadeInLightsRoutine());

        // 并行动作
        StartCoroutine(MoveLocalWithDelay(mazeTopChairGroup, chairUpOffset, chairMoveTime, chairDelay));
        StartCoroutine(MazeMovesRoutine());

        // 灯：如果你想“从演出开始算”也能灭，就可在此启动（一般不需要）
        if (fadeOutLights && lightsFadeOutDelayFromCinematicStart)
            StartCoroutine(FadeOutLightsRoutine(fromCinematicStart: true));

        // 相机演出
        StartCoroutine(CameraCinematicRoutine());
    }

    void CachePlayerCameraPose()
    {
        if (playerMainCameraTransform == null && targetCamera != null)
            playerMainCameraTransform = targetCamera.transform;

        _cachedPlayerPivot = playerCameraPivot;
        _cachedPlayerCam = playerMainCameraTransform;

        if (_cachedPlayerCam != null)
        {
            _cachedPlayerCamLocalPos = _cachedPlayerCam.localPosition;
            _cachedPlayerCamLocalRot = _cachedPlayerCam.localRotation;
        }
    }

    void CacheLightOriginalsIfNeeded()
    {
        if (controlledLights == null) return;
        if (_cachedOriginalIntensities != null && _cachedOriginalIntensities.Length == controlledLights.Length) return;

        _cachedOriginalIntensities = new float[controlledLights.Length];
        for (int i = 0; i < controlledLights.Length; i++)
        {
            var L = controlledLights[i];
            _cachedOriginalIntensities[i] = (L != null) ? L.intensity : 0f;
        }
    }

    void ForceLightsIntensity(float intensity, bool disable)
    {
        if (controlledLights == null || controlledLights.Length == 0) return;

        for (int i = 0; i < controlledLights.Length; i++)
        {
            var L = controlledLights[i];
            if (L == null) continue;
            L.enabled = true; // 先确保可用
            L.intensity = intensity;
            if (disable) L.enabled = false;
        }
    }

    IEnumerator FadeInLightsRoutine()
    {
        if (controlledLights == null || controlledLights.Length == 0) yield break;

        if (lightsFadeInDelay > 0f)
            yield return new WaitForSeconds(lightsFadeInDelay);

        // 从当前 intensity（通常 0）淡入到目标强度
        float[] startInt = new float[controlledLights.Length];
        for (int i = 0; i < controlledLights.Length; i++)
        {
            var L = controlledLights[i];
            if (L == null) { startInt[i] = 0f; continue; }

            L.enabled = true;
            L.intensity = beforeFadeInLightIntensity;
            startInt[i] = beforeFadeInLightIntensity;
        }


        float dur = Mathf.Max(0.01f, lightsFadeInDuration);
        float t = 0f;

        if (logLights) Debug.Log($"[Scene6IntroController] FadeInLights START dur={dur} target={lightsFadeInTargetIntensity}", this);

        while (t < dur)
        {
            t += Time.deltaTime;
            float k = SmoothStep01(Mathf.Clamp01(t / dur));

            for (int i = 0; i < controlledLights.Length; i++)
            {
                var L = controlledLights[i];
                if (L == null) continue;
                L.intensity = Mathf.Lerp(startInt[i], lightsFadeInTargetIntensity, k);
            }

            yield return null;
        }

        for (int i = 0; i < controlledLights.Length; i++)
        {
            var L = controlledLights[i];
            if (L == null) continue;
            L.intensity = lightsFadeInTargetIntensity;
        }

        if (logLights) Debug.Log("[Scene6IntroController] FadeInLights END", this);
    }

    IEnumerator FadeOutLightsRoutine(bool fromCinematicStart)
    {


        if (controlledLights == null || controlledLights.Length == 0) yield break;


        // —— 兜底：FadeIn 启动前，强制再黑一次（防止中途被改）——
        ForceLightsToBeforeFadeInState();

        if (lightsFadeInDelay > 0f)
            yield return new WaitForSeconds(lightsFadeInDelay);

        // 再锁一次，确保 delay 期间也不会亮
        ForceLightsToBeforeFadeInState();

        // ↓↓↓ 原来的 FadeIn 逻辑从这里开始 ↓↓↓

        if (fromCinematicStart)
        {
            float targetTime = _cinematicStartTime + Mathf.Max(0f, lightsFadeOutDelay);
            while (Time.time < targetTime) yield return null;
        }
        else
        {
            if (lightsFadeOutDelay > 0f) yield return new WaitForSeconds(lightsFadeOutDelay);
        }

        float[] startInt = new float[controlledLights.Length];
        bool anyValid = false;

        for (int i = 0; i < controlledLights.Length; i++)
        {
            var L = controlledLights[i];
            if (L == null) { startInt[i] = 0f; continue; }
            startInt[i] = L.intensity;
            anyValid = true;
        }

        if (!anyValid) yield break;

        float dur = Mathf.Max(0.01f, lightsFadeOutDuration);
        float t = 0f;

        if (logLights) Debug.Log($"[Scene6IntroController] FadeOutLights START dur={dur} target={lightsFadeOutTargetIntensity}", this);

        while (t < dur)
        {
            t += Time.deltaTime;
            float k = SmoothStep01(Mathf.Clamp01(t / dur));

            for (int i = 0; i < controlledLights.Length; i++)
            {
                var L = controlledLights[i];
                if (L == null) continue;
                L.intensity = Mathf.Lerp(startInt[i], lightsFadeOutTargetIntensity, k);
            }

            yield return null;
        }

        for (int i = 0; i < controlledLights.Length; i++)
        {
            var L = controlledLights[i];
            if (L == null) continue;
            L.intensity = lightsFadeOutTargetIntensity;
            if (disableLightsAfterFadeOut) L.enabled = false;
        }

        if (logLights) Debug.Log("[Scene6IntroController] FadeOutLights END", this);
    }

    IEnumerator MazeMovesRoutine()
    {
        if (mazeUnifiedDelay > 0f)
            yield return new WaitForSeconds(mazeUnifiedDelay);

        foreach (var m in mazeMoves)
        {
            if (m == null || m.target == null) continue;
            float d = Mathf.Max(0f, m.extraDelay);
            StartCoroutine(MoveLocalWithDelay(m.target, m.localOffset, m.moveTime, d));
        }
    }

    IEnumerator CameraCinematicRoutine()
    {
        Transform cam = targetCamera.transform;

        if (startUpsideDown)
            ApplyRollAbsolute(cam, upsideDownRollDegrees);

        if (pathADelay > 0f) yield return new WaitForSeconds(pathADelay);
        if (pathA != null && pathA.Count >= 2 && pathADuration > 0f)
            yield return MoveCameraAlongPath(cam, pathA, pathADuration, allowLookAt: true);

        if (useTransitionAtoB && pathA != null && pathA.Count >= 2 && pathB != null && pathB.Count >= 2)
        {
            if (transitionExtraDelay > 0f) yield return new WaitForSeconds(transitionExtraDelay);
            yield return TransitionSplineAndRoll_AtoB(cam);
        }

        if (pathBDelay > 0f) yield return new WaitForSeconds(pathBDelay);
        if (pathB != null && pathB.Count >= 2 && pathBDuration > 0f)
            yield return MoveCameraAlongPath(cam, pathB, pathBDuration, allowLookAt: true);

        if (returnToPlayerAfterCinematic)
        {
            if (returnDelay > 0f) yield return new WaitForSeconds(returnDelay);
            yield return ReturnSplineToPlayer(cam);
        }

        // 灯：如果设置为“从演出结束后算 delay”，就在这里触发
        if (fadeOutLights && !lightsFadeOutDelayFromCinematicStart)
            StartCoroutine(FadeOutLightsRoutine(fromCinematicStart: false));

        if (lockMouseLook) SetMouseLookLock(false);
    }


    // =========================
    // Polyline fallback (constant speed along straight segments)
    // Fix for: CS0103 MoveCameraAlongPolylineConstantSpeed not found
    // =========================
    private IEnumerator MoveCameraAlongPolylineConstantSpeed(
        Transform cam,
        List<Vector3> pts,
        float dur,
        bool allowLookAt = true
    )
    {
        if (cam == null || pts == null || pts.Count < 2) yield break;

        // segment lengths
        float totalLen = 0f;
        float[] segLen = new float[pts.Count - 1];
        for (int i = 0; i < pts.Count - 1; i++)
        {
            segLen[i] = Vector3.Distance(pts[i], pts[i + 1]);
            totalLen += segLen[i];
        }
        if (totalLen <= 0.0001f) yield break;

        cam.position = pts[0];
        ApplyLookAtIfNeeded_Safe(cam, cam.position, allowLookAt);

        float t = 0f;
        float duration = Mathf.Max(0.01f, dur);

        while (t < duration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / duration);

            // 如果你的脚本里有 strictConstantSpeed 变量，就会用它
            // 否则默认用 SmoothStep，让移动更“爽”
            bool useSmooth = true;
            try { useSmooth = !strictConstantSpeed; } catch { useSmooth = true; }
            if (useSmooth) u = SmoothStep01(u);

            float targetDist = u * totalLen;
            Vector3 pos = EvaluatePolylineByDistance(pts, segLen, targetDist);

            cam.position = pos;
            ApplyLookAtIfNeeded_Safe(cam, pos, allowLookAt);

            yield return null;
        }

        cam.position = pts[pts.Count - 1];
        ApplyLookAtIfNeeded_Safe(cam, cam.position, allowLookAt);
    }

    private Vector3 EvaluatePolylineByDistance(List<Vector3> pts, float[] segLen, float targetDist)
    {
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

    // 安全 LookAt：优先调用你脚本里现有的 ApplyLookAtIfNeeded；没有就用 fallback
    private void ApplyLookAtIfNeeded_Safe(Transform cam, Vector3 camPos, bool allow)
    {
        if (!allow || cam == null) return;

        // 如果你原脚本里已经有 ApplyLookAtIfNeeded(...)（不同签名也可能），这里尝试调用
        // 由于 C# 不支持反射直接调用私有方法且会拖慢，这里用最稳的 fallback：
        // 仅在 alwaysLookAtSingleTarget && alwaysLookAtTarget 存在时 LookAt。
        try
        {
            if (alwaysLookAtSingleTarget && alwaysLookAtTarget != null)
            {
                Vector3 dir = alwaysLookAtTarget.position - camPos;
                if (dir.sqrMagnitude > 0.0001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);

                    float lerp = 1f;
                    try
                    {
                        lerp = (1f - Mathf.Exp(-lookAtSmooth * Time.deltaTime)) * Mathf.Clamp01(lookAtStrength);
                    }
                    catch { lerp = 1f - Mathf.Exp(-10f * Time.deltaTime); }

                    cam.rotation = Quaternion.Slerp(cam.rotation, targetRot, lerp);
                }
            }
        }
        catch
        {
            // ignore any missing fields
        }
    }

    IEnumerator TransitionSplineAndRoll_AtoB(Transform cam)
    {
        Vector3 p0 = pathA[pathA.Count - 2].position;
        Vector3 p1 = pathA[pathA.Count - 1].position;
        Vector3 p2 = pathB[0].position;
        Vector3 p3 = pathB[1].position;

        Quaternion startRot = cam.rotation;
        Quaternion endRot = Quaternion.AngleAxis(transitionRollDegrees, cam.forward) * startRot;

        float dur = Mathf.Max(0.01f, transitionDuration);

        BuildSingleCatmullSegmentSamples(p0, p1, p2, p3, samplesPerSegment,
            out List<Vector3> samples, out List<float> cumulativeDist);

        if (samples.Count < 2) yield break;

        float totalLen = cumulativeDist[cumulativeDist.Count - 1];
        if (totalLen <= 0.0001f) yield break;

        float t = 0f;

        cam.position = samples[0];
        ApplyLookAtIfNeeded(cam, cam.position, allow: lookAtDuringTransition);

        while (t < dur)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / dur);
            if (!strictConstantSpeed) u = SmoothStep01(u);

            float targetDist = u * totalLen;
            Vector3 pos = EvaluateByDistance(samples, cumulativeDist, targetDist);

            cam.position = pos;
            cam.rotation = Quaternion.Slerp(startRot, endRot, u);
            ApplyLookAtIfNeeded(cam, pos, allow: lookAtDuringTransition);

            yield return null;
        }

        cam.position = samples[samples.Count - 1];
        cam.rotation = endRot;
        ApplyLookAtIfNeeded(cam, cam.position, allow: lookAtDuringTransition);
    }

    IEnumerator ReturnSplineToPlayer(Transform cam)
    {
        if (_cachedPlayerCam == null) yield break;

        Vector3 targetPos = _cachedPlayerCam.position;
        Quaternion targetRot = _cachedPlayerCam.rotation;

        Vector3 bPrev = (pathB != null && pathB.Count >= 2) ? pathB[pathB.Count - 2].position : cam.position;
        Vector3 bLast = (pathB != null && pathB.Count >= 1) ? pathB[pathB.Count - 1].position : cam.position;

        Vector3 p0 = bPrev;
        Vector3 p1 = bLast;
        Vector3 p2 = targetPos;

        Vector3 dir = (targetPos - bLast);
        if (dir.sqrMagnitude < 0.0001f) dir = (bLast - bPrev);
        if (dir.sqrMagnitude < 0.0001f) dir = cam.forward;
        dir.Normalize();

        float tail = Mathf.Max(0.5f, Vector3.Distance(bLast, targetPos));
        Vector3 p3 = targetPos + dir * tail;

        float dur = Mathf.Max(0.01f, returnDuration);

        BuildSingleCatmullSegmentSamples(p0, p1, p2, p3, samplesPerSegment,
            out List<Vector3> samples, out List<float> cumulativeDist);

        if (samples.Count < 2) yield break;

        float totalLen = cumulativeDist[cumulativeDist.Count - 1];
        if (totalLen <= 0.0001f) yield break;

        Quaternion startRot = cam.rotation;

        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / dur);
            if (!strictConstantSpeed) u = SmoothStep01(u);

            float targetDist = u * totalLen;
            Vector3 pos = EvaluateByDistance(samples, cumulativeDist, targetDist);

            cam.position = pos;
            cam.rotation = Quaternion.Slerp(startRot, targetRot, u);
            ApplyLookAtIfNeeded(cam, pos, allow: lookAtDuringReturn);

            yield return null;
        }

        cam.position = targetPos;
        cam.rotation = targetRot;

        if (reparentToPivotOnFinish && _cachedPlayerPivot != null)
        {
            cam.SetParent(_cachedPlayerPivot, true);
            cam.localPosition = _cachedPlayerCamLocalPos;
            cam.localRotation = _cachedPlayerCamLocalRot;
        }
    }

    IEnumerator MoveLocalWithDelay(Transform target, Vector3 offset, float duration, float delay)
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
            float k = SmoothStep01(Mathf.Clamp01(t / duration));
            target.localPosition = Vector3.Lerp(start, end, k);
            yield return null;
        }

        target.localPosition = end;
    }

    IEnumerator MoveCameraAlongPath(Transform cam, List<Transform> waypoints, float totalDuration, bool allowLookAt)
    {
        if (waypoints == null || waypoints.Count < 2) yield break;

        var cps = new List<Vector3>(waypoints.Count);
        for (int i = 0; i < waypoints.Count; i++)
            cps.Add(waypoints[i] ? waypoints[i].position : Vector3.zero);

        float dur = Mathf.Max(0.01f, totalDuration);

        if (!useSmoothSplinePath)
        {
            yield return MoveCameraAlongPolylineConstantSpeed(cam, cps, dur, allowLookAt);
            yield break;
        }

        BuildSplineSamples(cps, samplesPerSegment, clampToWaypointEndpoints,
            out List<Vector3> samples, out List<float> cumulativeDist);

        if (samples.Count < 2) yield break;

        float totalLen = cumulativeDist[cumulativeDist.Count - 1];
        if (totalLen <= 0.0001f) yield break;

        cam.position = samples[0];
        ApplyLookAtIfNeeded(cam, cam.position, allowLookAt);

        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / dur);
            if (!strictConstantSpeed) u = SmoothStep01(u);

            float targetDist = u * totalLen;
            Vector3 pos = EvaluateByDistance(samples, cumulativeDist, targetDist);

            cam.position = pos;
            ApplyLookAtIfNeeded(cam, pos, allowLookAt);
            yield return null;
        }

        cam.position = samples[samples.Count - 1];
        ApplyLookAtIfNeeded(cam, cam.position, allowLookAt);
    }

    void BuildSplineSamples(
        List<Vector3> controlPoints,
        int samplesPerSeg,
        bool clampEndpoints,
        out List<Vector3> samples,
        out List<float> cumulativeDist)
    {
        samples = new List<Vector3>();
        cumulativeDist = new List<float>();

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
                if (i > 0 && s == 0) continue;
                float t = (float)s / (steps - 1);
                samples.Add(CatmullRom(p0, p1, p2, p3, t));
            }
        }

        cumulativeDist.Add(0f);
        float acc = 0f;
        for (int i = 1; i < samples.Count; i++)
        {
            acc += Vector3.Distance(samples[i - 1], samples[i]);
            cumulativeDist.Add(acc);
        }
    }

    void BuildSingleCatmullSegmentSamples(
        Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3,
        int samplesCount,
        out List<Vector3> samples,
        out List<float> cumulativeDist)
    {
        samples = new List<Vector3>(Mathf.Max(2, samplesCount));
        cumulativeDist = new List<float>(Mathf.Max(2, samplesCount));

        int steps = Mathf.Max(2, samplesCount);
        for (int s = 0; s < steps; s++)
        {
            float t = (float)s / (steps - 1);
            samples.Add(CatmullRom(p0, p1, p2, p3, t));
        }

        cumulativeDist.Add(0f);
        float acc = 0f;
        for (int i = 1; i < samples.Count; i++)
        {
            acc += Vector3.Distance(samples[i - 1], samples[i]);
            cumulativeDist.Add(acc);
        }
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
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

    Vector3 EvaluateByDistance(List<Vector3> samples, List<float> cumulativeDist, float targetDist)
    {
        if (targetDist <= 0f) return samples[0];
        float totalLen = cumulativeDist[cumulativeDist.Count - 1];
        if (targetDist >= totalLen) return samples[samples.Count - 1];

        int lo = 0;
        int hi = cumulativeDist.Count - 1;
        while (lo < hi)
        {
            int mid = (lo + hi) / 2;
            if (cumulativeDist[mid] < targetDist) lo = mid + 1;
            else hi = mid;
        }

        int i1 = Mathf.Clamp(lo, 1, cumulativeDist.Count - 1);
        int i0 = i1 - 1;

        float d0 = cumulativeDist[i0];
        float d1 = cumulativeDist[i1];
        float seg = Mathf.Max(0.0001f, d1 - d0);
        float t = (targetDist - d0) / seg;

        return Vector3.Lerp(samples[i0], samples[i1], t);
    }

    void ApplyLookAtIfNeeded(Transform cam, Vector3 camPos, bool allow)
    {
        if (!allow) return;
        if (!alwaysLookAtSingleTarget) return;
        if (alwaysLookAtTarget == null) return;

        Vector3 dir = alwaysLookAtTarget.position - camPos;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);

        if (keepRollWhileLooking)
        {
            float currentRoll = cam.eulerAngles.z;
            Vector3 fwd = (targetRot * Vector3.forward).normalized;
            targetRot = Quaternion.AngleAxis(currentRoll, fwd) * targetRot;
        }

        float lerp = (1f - Mathf.Exp(-lookAtSmooth * Time.deltaTime)) * Mathf.Clamp01(lookAtStrength);
        cam.rotation = Quaternion.Slerp(cam.rotation, targetRot, lerp);
    }

    void ApplyRollAbsolute(Transform cam, float rollDeg)
    {
        var e = cam.eulerAngles;
        e.z = rollDeg;
        cam.eulerAngles = e;
    }

    float SmoothStep01(float x) => x * x * (3f - 2f * x);

    void SetMouseLookLock(bool locked)
    {
        if (mouseLookScriptsToDisable != null)
        {
            foreach (var s in mouseLookScriptsToDisable)
                if (s != null) s.enabled = !locked;
        }

        if (lockCursor)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
