using System.Collections;
using UnityEngine;

public class Scene2GravityOverride : MonoBehaviour
{
    [Header("Gravity Override")]
    public PlayerMove player;
    public float heavyGravity = 2000f;
    public float normalGravity = 20f;
    public float duration = 12f;

    private bool _triggered = false;

    private void Start()
    {
        if (!_triggered)
        {
            StartCoroutine(ApplyGravityOverride());
        }
    }

    private IEnumerator ApplyGravityOverride()
    {
        _triggered = true;

        if (player == null)
        {
            Debug.LogError("[Scene2GravityOverride] Player is NULL");
            yield break;
        }

        // 1️⃣ 强制大重力
        player.SetGravity(heavyGravity);

        // 可选：防止一开始残留向上速度
        // 让玩家立刻“贴地”
        // ⚠ 不破坏后续跳跃
        yield return null;

        // 2️⃣ 等待动画时长
        yield return new WaitForSeconds(duration);

        // 3️⃣ 恢复正常重力
        player.SetGravity(normalGravity);
    }
}
