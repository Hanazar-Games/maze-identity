
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToOptionsFromPause : MonoBehaviour
{
    [Header("Scenes")]
    public string optionsSceneName = "Options";

    [Header("Player & Camera")]
    [Tooltip("拖玩家的 Transform（最好直接拖 PlayerRoot）。如果不想拖，也可以用 Tag 查找。")]
    public Transform playerTransform;

    [Tooltip("可选：第一人称相机 Transform（用于保存 pitch）。")]
    public Transform cameraTransform;

    [Tooltip("如果 playerTransform 为空，就用 tag 找。")]
    public string playerTag = "Player";

    [Header("Return Behavior")]
    public bool reopenPauseOnReturn = true;

    [Tooltip("从 Options 返回原 Scene 后，黑场淡出（FadeIn）时长。")]
    public float returnFadeInSeconds = 1.0f;

    [Header("Debug")]
    public bool verboseDebug = true;

    // 两套 key（你项目里同时出现过）
    private const string KEY_RETURN_SCENE_NEW = "MI_OptionsReturnScene"; // OptionsFader 读这个
    private const string KEY_RETURN_SCENE_OLD = "MI_ReturnScene";        // 旧脚本读这个（你的日志也打印它）
    private const string KEY_RETURN_TO_PAUSE = "MI_ReturnFromOptionsToPause";

    public void OnOptionsClicked()
    {
        string activeScene = SceneManager.GetActiveScene().name;

        if (verboseDebug)
        {
            Debug.Log("[GoToOptionsFromPause] OnOptionsClicked. ActiveScene=" + activeScene);
            Debug.Log("[GoToOptionsFromPause] BEFORE write: " +
                      KEY_RETURN_SCENE_NEW + "=" + PlayerPrefs.GetString(KEY_RETURN_SCENE_NEW, "<EMPTY>") + " | " +
                      KEY_RETURN_SCENE_OLD + "=" + PlayerPrefs.GetString(KEY_RETURN_SCENE_OLD, "<EMPTY>") + " | " +
                      KEY_RETURN_TO_PAUSE + "=" + PlayerPrefs.GetInt(KEY_RETURN_TO_PAUSE, -1));
        }

        // 0) 防呆：如果你在 Options 场景里点这个，会把返回写成 Options 自己
        if (activeScene == optionsSceneName)
        {
            Debug.LogError("[GoToOptionsFromPause] This script should NOT be used inside Options scene. Abort.");
            return;
        }

        // 1) 找 player
        Transform p = playerTransform;
        if (p == null)
        {
            var go = GameObject.FindGameObjectWithTag(playerTag);
            if (go != null) p = go.transform;
        }

        // 2) 确保 SnapshotManager 存在（避免 Instance 为空导致 Capture 根本没执行）
        EnsureSnapshotManager();

        // 3) 捕获快照（scene + pos/rot）
        if (OptionsSnapshotManager.Instance != null && p != null)
        {
            OptionsSnapshotManager.Instance.Capture(
                activeScene,
                p,
                cameraTransform,
                reopenPauseOnReturn,
                returnFadeInSeconds
            );
        }
        else
        {
            Debug.LogError("[GoToOptionsFromPause] Snapshot capture failed. " +
                           "OptionsSnapshotManager.Instance=" + (OptionsSnapshotManager.Instance == null ? "NULL" : "OK") +
                           " player=" + (p == null ? "NULL" : p.name));
        }

        // 4) ★关键：写入“返回场景”到 PlayerPrefs（两套都写，彻底兼容）
        PlayerPrefs.SetString(KEY_RETURN_SCENE_NEW, activeScene);
        PlayerPrefs.SetString(KEY_RETURN_SCENE_OLD, activeScene);
        PlayerPrefs.SetInt(KEY_RETURN_TO_PAUSE, reopenPauseOnReturn ? 1 : 0);
        PlayerPrefs.Save();

        if (verboseDebug)
        {
            Debug.Log("[GoToOptionsFromPause] AFTER write: " +
                      KEY_RETURN_SCENE_NEW + "=" + PlayerPrefs.GetString(KEY_RETURN_SCENE_NEW, "<EMPTY>") + " | " +
                      KEY_RETURN_SCENE_OLD + "=" + PlayerPrefs.GetString(KEY_RETURN_SCENE_OLD, "<EMPTY>") + " | " +
                      KEY_RETURN_TO_PAUSE + "=" + PlayerPrefs.GetInt(KEY_RETURN_TO_PAUSE, -1));
        }

        // 5) 切 Options 前：解锁鼠标 + 恢复时间
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 6) 进 Options
        if (!string.IsNullOrEmpty(optionsSceneName))
        {
            if (verboseDebug) Debug.Log("[GoToOptionsFromPause] LoadScene(" + optionsSceneName + ")");
            SceneManager.LoadScene(optionsSceneName);
        }
        else
        {
            Debug.LogError("[GoToOptionsFromPause] optionsSceneName is empty.");
        }
    }

    private void EnsureSnapshotManager()
    {
        if (OptionsSnapshotManager.Instance != null) return;

        // 自动创建一个（防止你忘记放进场景）
        var go = new GameObject("OptionsSnapshotManager");
        go.AddComponent<OptionsSnapshotManager>();
        // OptionsSnapshotManager 自己应当 DontDestroyOnLoad
        if (verboseDebug) Debug.Log("[GoToOptionsFromPause] Created OptionsSnapshotManager (auto).");
    }
}
