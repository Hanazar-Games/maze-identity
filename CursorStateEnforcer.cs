using UnityEngine;

public class CursorStateEnforcer : MonoBehaviour
{
    [Header("Target Cursor State")]
    public bool cursorVisible = true;
    public CursorLockMode lockMode = CursorLockMode.None;

    [Header("Also fix common UI blockers")]
    public bool resetTimeScale = true;

    [Tooltip("有些项目会在第一帧又被别的脚本锁回去，所以这里多执行几帧。")]
    [Range(1, 10)] public int enforceFrames = 3;

    private void OnEnable()
    {
        if (resetTimeScale) Time.timeScale = 1f;
        Apply();
    }

    private void Start()
    {
        // 多执行几帧，防止切场景后某个脚本在 Start/OnEnable 又锁回去
        StartCoroutine(EnforceRoutine());
    }

    private System.Collections.IEnumerator EnforceRoutine()
    {
        for (int i = 0; i < enforceFrames; i++)
        {
            Apply();
            yield return null;
        }
    }

    private void Apply()
    {
        Cursor.lockState = lockMode;
        Cursor.visible = cursorVisible;
    }
}
