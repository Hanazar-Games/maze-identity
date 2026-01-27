using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class Scene3To4ShakeTrigger : MonoBehaviour
{
    [Header("References")]
    public PlayerMove player;                 // 拖 Player 上的 PlayerMove
    public CharacterController playerCC;      // 可不填，会自动从 player 获取
    public Transform cameraToShake;           // 建议拖 player.cameraPivot（更稳）或 cameraTransform
    public Transform directionRef;            // 推进方向参考（用它的 forward）。建议放一个空物体 Arrow

    public bool triggerOnce = true;

    [Header("Lock Control")]
    public bool lockMovement = true;
    public bool lockMouseLook = true;

    [Header("Auto Acceleration (No Speed Cap)")]
    public bool enableAutoPush = true;
    public float acceleration = 25f;          // 可调：加速度（m/s^2）
    public float startSpeed = 0f;             // 初速度
    public float pushDuration = 2.0f;         // 推进持续时间（一般>=震动时间）
    public bool alignFacingToPushDir = true;  // 触发后锁玩家面向推进方向

    [Header("Shake")]
    public float shakeDuration = 2.0f;
    public float amplitude = 0.12f;
    public float frequency = 18f;
    public AnimationCurve fadeOut = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Fog (Optional)")]
    public bool setFogToZeroAfterDelay = true;
    public float fogDelay = 4f;
    public float fogTargetDensity = 0f;
    public bool forceFogEnabled = true;

    [Header("Scene Transition")]
    public bool autoLoadScene = true;
    public string nextSceneName = "Scene4";
    public float loadDelay = 2.2f;

    private bool _triggered;
    private Vector3 _camLocalStart;
    private float _currentSpeed;

    private void Awake()
    {
        // 确保是 Trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Start()
    {
        if (cameraToShake != null)
            _camLocalStart = cameraToShake.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered && triggerOnce) return;
        if (!other.CompareTag("Player")) return;

        if (player == null)
            player = other.GetComponentInParent<PlayerMove>();

        if (player == null)
        {
            Debug.LogError("[Scene3To4ShakeTrigger] PlayerMove not found. Drag it in Inspector.");
            return;
        }

        if (playerCC == null)
            playerCC = player.GetComponent<CharacterController>();

        if (playerCC == null)
        {
            Debug.LogError("[Scene3To4ShakeTrigger] CharacterController not found on Player.");
            return;
        }

        if (cameraToShake == null)
        {
            // 优先用 pivot（如果你没拖，就用 cameraTransform）
            cameraToShake = player.cameraPivot != null ? player.cameraPivot : player.cameraTransform;
            if (cameraToShake == null) cameraToShake = Camera.main.transform;
        }

        _camLocalStart = cameraToShake.localPosition;

        if (directionRef == null)
        {
            // 默认用这个触发器物体本身的 forward 作为方向
            directionRef = transform;
        }

        _triggered = true;
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        if (setFogToZeroAfterDelay)
            StartCoroutine(SetFogAfterDelay());

        // 1) 锁输入（避免玩家干扰）
        if (lockMovement) player.enableMovement = false;
        if (lockMouseLook) player.enableMouseLook = false;

        // 2) 计算推进方向（只用水平面方向，避免向上/向下）
        Vector3 dir = directionRef.forward;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector3.right; // 兜底
        dir.Normalize();

        // 3) 对齐玩家朝向到推进方向（解决“锁方向不对”）
        if (alignFacingToPushDir)
        {
            Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
            player.transform.rotation = look;
        }

        // 4) 并行：推进 + 震动
        _currentSpeed = Mathf.Max(0f, startSpeed);

        Coroutine pushCo = null;
        if (enableAutoPush && pushDuration > 0f)
            pushCo = StartCoroutine(AutoPush(dir));

        if (cameraToShake != null && shakeDuration > 0f)
            yield return StartCoroutine(ShakeLocal(cameraToShake));

        // 等推进结束（如果推进比震动长）
        if (pushCo != null)
        {
            // 等到 pushDuration 结束
            yield return new WaitForSeconds(Mathf.Max(0f, pushDuration - shakeDuration));
        }

        // 5) 恢复相机位置
        if (cameraToShake != null)
            cameraToShake.localPosition = _camLocalStart;

        // 6) 切场（可选）
        if (autoLoadScene)
        {
            float remain = Mathf.Max(0f, loadDelay - Mathf.Max(shakeDuration, pushDuration));
            if (remain > 0f) yield return new WaitForSeconds(remain);
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator AutoPush(Vector3 dir)
    {
        float t = 0f;
        while (t < pushDuration)
        {
            t += Time.deltaTime;

            // 无上限加速
            _currentSpeed += acceleration * Time.deltaTime;

            // 直接推 CharacterController（不依赖 PlayerMove 的速度上限）
            Vector3 move = dir * (_currentSpeed * Time.deltaTime);
            playerCC.Move(move);

            yield return null;
        }
    }

    private IEnumerator SetFogAfterDelay()
    {
        yield return new WaitForSeconds(fogDelay);

        if (forceFogEnabled) RenderSettings.fog = true;

        // 你项目如果用的是 Exponential/Exp2，这个就是密度
        // 如果用 Linear，也不会报错，但要靠 start/end distance 控制；你这里要“变为0”，直接密度=0即可达到“无雾”效果
        RenderSettings.fogDensity = fogTargetDensity;
    }

    private IEnumerator ShakeLocal(Transform cam)
    {
        float t = 0f;
        while (t < shakeDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / Mathf.Max(0.0001f, shakeDuration));
            float w = fadeOut != null ? fadeOut.Evaluate(k) : (1f - k);

            float s = Mathf.Sin(t * frequency);
            float c = Mathf.Cos(t * frequency * 0.93f);

            Vector3 offset = new Vector3(s, c, 0f) * (amplitude * w);
            cam.localPosition = _camLocalStart + offset;

            yield return null;
        }
    }
}
