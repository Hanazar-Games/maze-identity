using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    [Header("Scene")]
    public string nextSceneName = "Scene2";

    [Header("Rail Path (Waypoints in order)")]
    public Transform[] waypoints;
    public float arriveDistance = 0.35f;

    [Header("Rail Motion")]
    public bool lockMovement = true;
    public bool lockMouseLook = true;

    public float startSpeed = 6f;         // 起步速度
    public float acceleration = 18f;      // 持续加速（越大越疯）
    public float maxSpeed = 45f;          // 最高速度上限
    public float endSnapSpeed = 70f;      // 接近终点时可允许更快（可选）
    public float gravityDownForce = 3f;   // 防止悬空的下压（根据你的场景可调）

    [Header("External Shake (ramp up with progress)")]
    [Range(0f, 1f)] public float shakeMinStrength = 0.10f;
    [Range(0f, 1f)] public float shakeMaxStrength = 1.00f;

    public float shakeMinFreq = 8f;
    public float shakeMaxFreq = 32f;

    public float shakeMinPosAmp = 0.02f;
    public float shakeMaxPosAmp = 0.11f;
    public float shakeMinRollAmp = 1.0f;
    public float shakeMaxRollAmp = 5.5f;

    [Header("FOV Boost (Start Trigger)")]
    public bool enableFovBoost = true;
    public float fovTarget = 80f;        // StartTrigger 后拉到多大
    public float fovLerpSpeed = 2.5f;    // 拉大速度

    [Header("FOV Extra Ramp (Optional, with progress)")]
    public bool enableFovRampWithProgress = false;
    public float fovExtraAtEnd = 10f;    // 进度到 1 时额外再加多少（例如 +10）
    public float fovRampWeight = 1.0f;   // 额外FOV影响权重

    [Header("End Transition")]
    public float endLoadDelay = 0.05f;   // 黑盒里切场景，几乎瞬间即可

    private bool started = false;
    private bool ending = false;

    private PlayerMove pm;
    private PlayerMovingEffect movingEffect;
    private CharacterController cc;

    private bool moveOrig, lookOrig;
    private Coroutine railRoutine;

    // FOV runtime
    private Camera playerCam;
    private float fovOriginal;
    private bool fovBoosting = false;
    private float fovProgress01 = 0f; // rail 进度（0..1）

    public void OnStartTriggerEnter(Collider other)
    {
        if (started) return;
        if (!other.CompareTag("Player")) return;

        started = true;

        pm = other.GetComponent<PlayerMove>();
        cc = other.GetComponent<CharacterController>();
        movingEffect = other.GetComponentInChildren<PlayerMovingEffect>(true);

        if (pm == null)
        {
            Debug.LogError("[StartTrigger] PlayerMove not found on Player.");
            return;
        }
        if (cc == null)
        {
            Debug.LogError("[StartTrigger] CharacterController not found on Player.");
            return;
        }
        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogError("[StartTrigger] Waypoints not set (need at least 2).");
            return;
        }

        // 记录原始输入开关
        moveOrig = pm.enableMovement;
        lookOrig = pm.enableMouseLook;

        // 锁输入（你要“锁住玩家移动/视角”）
        if (lockMovement) pm.enableMovement = false;
        if (lockMouseLook) pm.enableMouseLook = false;

        // 启动相机抖动：先小
        if (movingEffect != null)
        {
            movingEffect.SetExternalShakeModeRandom2D(true);
            movingEffect.StartExternalShake(shakeMinStrength);
            movingEffect.SetExternalShakeFreq(shakeMinFreq);
            movingEffect.SetExternalShakeAmps(shakeMinPosAmp, shakeMinRollAmp);
        }
        else
        {
            Debug.LogWarning("[StartTrigger] PlayerMovingEffect not found in Player children.");
        }

        // === FOV Boost start (Scene1->2 only) ===
        if (enableFovBoost)
        {
            // 找相机（优先用 movingEffect.mainCam，其次从Player子物体找）
            if (playerCam == null)
            {
                if (movingEffect != null && movingEffect.mainCam != null)
                    playerCam = movingEffect.mainCam;

                if (playerCam == null)
                    playerCam = other.GetComponentInChildren<Camera>(true);
            }

            if (playerCam != null)
            {
                fovOriginal = playerCam.fieldOfView;   // 通常是 51
                fovBoosting = true;

                // 关键：过场期间，暂时禁止 PlayerMovingEffect 每帧把FOV拉回
                if (movingEffect != null)
                    movingEffect.SetFovKickEnabled(false);
            }
            else
            {
                Debug.LogWarning("[StartTrigger] Camera not found under Player.");
            }
        }

        // 启动轨道推进（持续加速）
        railRoutine = StartCoroutine(RailPushCoroutine(other.transform));
        Debug.Log("[StartTrigger] RailPush ON (locked input, accelerate, shake ramp, FOV boost)");
    }

    public void OnEndTriggerEnter(Collider other)
    {
        if (ending) return;
        if (!other.CompareTag("Player")) return;

        ending = true;
        Debug.Log("[EndTrigger] Enter black box, keep shake, load next scene");

        if (railRoutine != null)
            StopCoroutine(railRoutine);

        // === Restore FOV control back to PlayerMovingEffect ===
        if (playerCam != null)
            playerCam.fieldOfView = fovOriginal;  // 立刻回到51（进黑盒看不出来突兀）

        fovBoosting = false;

        if (movingEffect != null)
            movingEffect.SetFovKickEnabled(true); // 还回它的控制权（它会保持51）

        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private void LateUpdate()
    {
        if (!fovBoosting) return;
        if (playerCam == null) return;

        float target = fovTarget;

        // 可选：FOV 随 rail 进度再额外拉大一点
        if (enableFovRampWithProgress)
        {
            float extra = Mathf.Lerp(0f, fovExtraAtEnd, EaseIn(fovProgress01)) * Mathf.Max(0f, fovRampWeight);
            target += extra;
        }

        playerCam.fieldOfView = Mathf.Lerp(
            playerCam.fieldOfView,
            target,
            Time.deltaTime * Mathf.Max(0.01f, fovLerpSpeed)
        );
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(endLoadDelay);
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator RailPushCoroutine(Transform playerTf)
    {
        int idx = 0;
        float speed = Mathf.Max(0.1f, startSpeed);

        int segmentCount = Mathf.Max(1, waypoints.Length - 1);
        fovProgress01 = 0f;

        // 朝向第一段方向（可选）
        Vector3 initialDir = (waypoints[1].position - waypoints[0].position);
        initialDir.y = 0f;
        if (initialDir.sqrMagnitude > 0.0001f)
            playerTf.rotation = Quaternion.LookRotation(initialDir.normalized, Vector3.up);

        while (!ending)
        {
            Transform targetWp = waypoints[Mathf.Clamp(idx + 1, 1, waypoints.Length - 1)];
            Vector3 toTarget = (targetWp.position - playerTf.position);

            Vector3 dir = toTarget;
            dir.y = 0f;

            float dist = dir.magnitude;

            // 到达下一点
            if (dist <= arriveDistance)
            {
                idx++;
                if (idx >= waypoints.Length - 1)
                    idx = waypoints.Length - 2;

                yield return null;
                continue;
            }

            dir /= Mathf.Max(0.0001f, dist);

            // 持续加速（一直加速中）
            speed += acceleration * Time.deltaTime;

            // 接近末端允许更疯（可选）
            float endBoost01 = Mathf.InverseLerp(waypoints.Length - 3, waypoints.Length - 2, idx);
            float maxNow = Mathf.Lerp(maxSpeed, endSnapSpeed, Mathf.Clamp01(endBoost01));
            speed = Mathf.Min(speed, maxNow);

            // 进度估算：按段推进（稳定）
            float distFactor = 1f - Mathf.InverseLerp(0f, 6f, dist);
            fovProgress01 = Mathf.Clamp01((idx + distFactor) / segmentCount);

            // === 抖动从小到大（强度/频率/幅度）===
            if (movingEffect != null)
            {
                float e = EaseIn(fovProgress01);

                float strength = Mathf.Lerp(shakeMinStrength, shakeMaxStrength, e);
                float freq = Mathf.Lerp(shakeMinFreq, shakeMaxFreq, e);
                float posAmp = Mathf.Lerp(shakeMinPosAmp, shakeMaxPosAmp, e);
                float rollAmp = Mathf.Lerp(shakeMinRollAmp, shakeMaxRollAmp, e);

                movingEffect.StartExternalShake(strength);
                movingEffect.SetExternalShakeFreq(freq);
                movingEffect.SetExternalShakeAmps(posAmp, rollAmp);
            }

            // === 推动玩家（CharacterController.Move 最稳）===
            Vector3 motion = dir * speed;
            motion.y = -gravityDownForce;

            cc.Move(motion * Time.deltaTime);

            // 让玩家朝向轨道方向（更像被拖拽）
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                playerTf.rotation = Quaternion.Slerp(playerTf.rotation, targetRot, Time.deltaTime * 8f);
            }

            yield return null;
        }
    }

    private static float EaseIn(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t;
    }

    // 如果未来 Scene2 还沿用同一个 Player，可以调用恢复输入
    public void RestorePlayerInput()
    {
        if (pm != null)
        {
            pm.enableMovement = moveOrig;
            pm.enableMouseLook = lookOrig;
        }

        if (movingEffect != null)
        {
            movingEffect.StopExternalShake();
            movingEffect.SetFovKickEnabled(true);
        }

        if (playerCam != null)
            playerCam.fieldOfView = fovOriginal;

        fovBoosting = false;
    }
}
