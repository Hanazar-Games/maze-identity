
using UnityEngine;

public class ClearContinueAtEnd : MonoBehaviour
{
    [Tooltip("进入该物体所在场景时自动清除 Continue（用于 SceneEND）。")]
    public bool clearOnStart = true;

    [Header("Debug")]
    public bool debugLogs = true;

    private void Start()
    {
        if (clearOnStart) ClearNow();
    }

    [ContextMenu("Clear Now")]
    public void ClearNow()
    {
        if (ProgressManager.Instance == null)
        {
            Debug.LogWarning("[ClearContinueAtEnd] ProgressManager missing. Nothing cleared.");
            return;
        }

        // ✅ 只清 Continue + 标记通关，不要 ClearSave()
        ProgressManager.Instance.MarkCompletedAndClearContinue();

        if (debugLogs)
        {
            var d = ProgressManager.Instance.GetData();
            Debug.Log("[ClearContinueAtEnd] MarkCompletedAndClearContinue called. " +
                      $"completed={(d != null && d.completed)} lastScene='{(d != null ? d.lastSceneName : "<null>")}'");
        }
    }
}
