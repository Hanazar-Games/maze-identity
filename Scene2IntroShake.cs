
using UnityEngine;

public class Scene2IntroShake : MonoBehaviour
{
    [Header("Skip When Returning From Options")]
    public bool skipWhenReturningFromOptions = true;
    public string returningKey = "MI_ReturningFromOptions"; // 和你 Options 系统一致的 key
    public bool debugLogs = false;

    [Header("Duration")]
    public float duration = 1.2f;

    [Header("Shake")]
    public float positionAmplitude = 0.03f; // 位移幅度（米）
    public float rotationAmplitude = 1.2f;  // 旋转幅度（度）
    public float frequency = 18f;

    private Vector3 startLocalPos;
    private Quaternion startLocalRot;
    private float t;
    private bool running = true;

    void Awake()
    {
        startLocalPos = transform.localPosition;
        startLocalRot = transform.localRotation;

        // ✅ 关键：从 Options 返回就跳过
        if (skipWhenReturningFromOptions && PlayerPrefs.GetInt(returningKey, 0) == 1)
        {
            if (debugLogs) Debug.Log($"[Scene2IntroShake] SKIP (key={returningKey}=1)");
            running = false;
            transform.localPosition = startLocalPos;
            transform.localRotation = startLocalRot;
            Destroy(this); // 保持你“用完即删”的风格
        }
    }

    void LateUpdate()
    {
        if (!running) return;

        t += Time.deltaTime;
        float k = Mathf.Clamp01(1f - t / Mathf.Max(0.01f, duration)); // 逐渐衰减到0

        float s1 = Mathf.Sin(Time.time * frequency);
        float s2 = Mathf.Sin(Time.time * frequency * 1.37f);

        Vector3 posOffset = new Vector3(s1, s2, 0f) * (positionAmplitude * k);
        Vector3 rotOffset = new Vector3(s2, s1, 0f) * (rotationAmplitude * k);

        transform.localPosition = startLocalPos + posOffset;
        transform.localRotation = startLocalRot * Quaternion.Euler(rotOffset);

        if (t >= duration)
        {
            running = false;
            transform.localPosition = startLocalPos;
            transform.localRotation = startLocalRot;
            Destroy(this); // 用完即删，只影响 Scene2 开头
        }
    }
}
