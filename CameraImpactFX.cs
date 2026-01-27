using UnityEngine;

public class CameraImpactFX : MonoBehaviour
{
    [Header("Refs")]
    public Transform cameraTransform;      // Main Camera
    public PlayerMove playerMove;          // 你的 PlayerMove（用来读 vy/grounded）

    [Header("Landing Kick")]
    public float maxKickDown = 0.08f;      // 最大下压米数（0.05~0.12）
    public float kickInTime = 0.05f;       // 压下速度
    public float kickOutTime = 0.15f;      // 回弹速度

    [Header("Trigger Threshold")]
    public float minImpactSpeed = 4f;      // 低于这个落地速度不触发
    public float maxImpactSpeed = 25f;     // 达到这个速度触发满额

    private float kickOffset;             // 当前相机Y偏移（负数向下）
    private float kickVel;

    private bool wasGrounded;

    void Reset()
    {
        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (!cameraTransform || !playerMove) return;

        bool grounded = playerMove.IsGroundedNow;     // 我下面教你在 PlayerMove 暴露这个
        float vy = playerMove.VerticalVelocity;       // 同上

        // 侦测落地沿：上一帧空中，这一帧落地
        if (grounded && !wasGrounded)
        {
            float impactSpeed = Mathf.Max(0f, -vy); // 下落速度取正
            if (impactSpeed >= minImpactSpeed)
            {
                float t = Mathf.InverseLerp(minImpactSpeed, maxImpactSpeed, impactSpeed);
                float kick = Mathf.Lerp(0f, maxKickDown, t);
                // 直接给一个瞬时目标（向下）
                kickOffset = -kick;
                kickVel = 0f;
            }
        }

        wasGrounded = grounded;

        // 平滑回弹到0（不依赖bug）
        float target = 0f;
        float smooth = (kickOffset < target) ? kickOutTime : kickInTime;
        kickOffset = Mathf.SmoothDamp(kickOffset, target, ref kickVel, Mathf.Max(0.01f, smooth));

        // 应用：只改 localPosition.y，不动其它
        var lp = cameraTransform.localPosition;
        lp.y += kickOffset;
        cameraTransform.localPosition = lp;
    }
}
