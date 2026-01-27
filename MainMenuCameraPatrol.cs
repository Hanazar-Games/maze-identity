using UnityEngine;

public class MainMenuCameraPatrol : MonoBehaviour
{
    [Header("Path")]
    public Transform pointA;
    public Transform pointB;

    [Header("Timing")]
    public float travelTime = 8f;          // 从A到B用时（秒）
    public float holdTimeAtEnds = 0.5f;    // 到端点停顿（秒）
    public bool useSmoothStep = true;

    [Header("Look")]
    public Transform lookTarget;           // 可选：一直看向某个目标
    public Vector3 lookOffset = Vector3.zero;

    private float _t;          // 0..1
    private int _dir = 1;      // 1=向B, -1=向A
    private float _holdTimer;

    private void Reset()
    {
        travelTime = 8f;
        holdTimeAtEnds = 0.5f;
        useSmoothStep = true;
    }

    private void LateUpdate()
    {
        if (pointA == null || pointB == null) return;
        if (travelTime <= 0.01f) travelTime = 0.01f;

        // 端点停顿
        if (_holdTimer > 0f)
        {
            _holdTimer -= Time.deltaTime;
            UpdateLook();
            return;
        }

        // 更新 t
        _t += (_dir * Time.deltaTime) / travelTime;

        // 到端点：夹紧 + 触发停顿 + 反向
        if (_t >= 1f)
        {
            _t = 1f;
            _holdTimer = holdTimeAtEnds;
            _dir = -1;
        }
        else if (_t <= 0f)
        {
            _t = 0f;
            _holdTimer = holdTimeAtEnds;
            _dir = 1;
        }

        float k = Mathf.Clamp01(_t);
        if (useSmoothStep) k = k * k * (3f - 2f * k);

        // 插值移动
        transform.position = Vector3.Lerp(pointA.position, pointB.position, k);

        UpdateLook();
    }

    private void UpdateLook()
    {
        if (lookTarget == null) return;

        Vector3 targetPos = lookTarget.position + lookOffset;
        Vector3 dir = (targetPos - transform.position);
        if (dir.sqrMagnitude < 0.0001f) return;

        // 平滑一点更舒服：你也可以改成 transform.LookAt(targetPos);
        Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 8f * Time.deltaTime);
    }
}
