using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("主菜单场景名（这里不会写 lastSceneName）")]
    public string mainMenuSceneName = "MainMenu";

    [Tooltip("是否在切换场景时自动尝试记录进度（只会记录 Scene1-6）")]
    public bool autoSaveOnSceneChanged = true;

    [Header("Continue Rules (Playable Scenes Only)")]
    [Tooltip("只允许写入进度的场景前缀（Scene1-Scene6）")]
    public string[] allowedScenePrefixes = new string[] { "Scene1", "Scene2", "Scene3", "Scene4", "Scene5", "Scene6" };

    [Tooltip("强排除关键字（无论大小写），这些场景永远不写入 lastSceneName")]
    public string[] blockedKeywords = new string[] { "options", "mainmenu", "transition", "end" };

    [Header("Debug")]
    public bool debugLogs = true;

    private SaveData _data;

    // =======================
    // BOOTSTRAP (关键修复点)
    // =======================
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        // 任何场景点 Play 都确保有 ProgressManager
        if (Instance != null) return;

        var existing = FindFirstObjectByType<ProgressManager>();
        if (existing != null)
        {
            Instance = existing;
            DontDestroyOnLoad(existing.gameObject);
            return;
        }

        var go = new GameObject("ProgressManager");
        Instance = go.AddComponent<ProgressManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        // 这里要非常小心：Bootstrap 可能已经创建了 Instance
        if (Instance != null && Instance != this)
        {
            if (debugLogs)
                Debug.LogWarning("[ProgressManager] Duplicate instance detected, destroying: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _data = SaveSystem.Load() ?? new SaveData();

        // 用 sceneLoaded 比 activeSceneChanged 更稳定（尤其是某些流程/插件）
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (debugLogs)
            Debug.Log("[ProgressManager] Awake OK. Loaded lastSceneName=" + (_data?.lastSceneName ?? "<NULL>"));
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!autoSaveOnSceneChanged) return;

        if (debugLogs)
            Debug.Log($"[ProgressManager] sceneLoaded: scene={scene.name}, mode={mode}, active={SceneManager.GetActiveScene().name}");

        // 进入主菜单不写入 lastSceneName
        if (string.Equals(scene.name, mainMenuSceneName, StringComparison.OrdinalIgnoreCase))
            return;

        // 只在“可玩场景”时记录
        TryRecordPlayableScene(scene.name, markStarted: true);
    }

    // =========================
    // 外部 API（MainMenu/Checkpoint 调用）
    // =========================

    public SaveData GetData() => _data;

    public bool HasProgressToContinue()
    {
        return _data != null
               && !_data.completed
               && !string.IsNullOrEmpty(_data.lastSceneName)
               && IsAllowedPlayableScene(_data.lastSceneName)
               && Application.CanStreamedLevelBeLoaded(_data.lastSceneName);
    }

    public bool TryRecordPlayableScene(string sceneName, bool markStarted)
    {
        if (string.IsNullOrWhiteSpace(sceneName)) return false;
        sceneName = sceneName.Trim();

        if (!IsAllowedPlayableScene(sceneName))
        {
            if (debugLogs)
                Debug.Log($"[ProgressManager] Not recorded (blocked): {sceneName}");
            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning($"[ProgressManager] Scene '{sceneName}' is not in Build Settings. Not recorded.");
            return false;
        }

        if (_data == null) _data = new SaveData();

        if (_data.completed)
        {
            _data.completed = false;
            if (debugLogs)
                Debug.Log("[ProgressManager] Completed cleared because a playable scene is recorded again.");
        }

        _data.lastSceneName = sceneName;

        if (markStarted)
            _data.hasStarted = true;

        SaveNow();

        if (debugLogs)
            Debug.Log($"[ProgressManager] Recorded lastSceneName => {sceneName}");

        return true;
    }

    public void SetCheckpoint(string checkpointId)
    {
        if (_data == null) _data = new SaveData();
        _data.lastCheckpointId = checkpointId ?? "";
        SaveNow();
    }

    public void MarkCompletedAndClearContinue()
    {
        if (_data == null) _data = new SaveData();
        _data.completed = true;
        _data.lastSceneName = "";
        _data.lastCheckpointId = "";
        SaveNow();

        if (debugLogs)
            Debug.Log("[ProgressManager] Completed = true, continue cleared.");
    }

    public void ClearSave()
    {
        _data = new SaveData();
        SaveSystem.DeleteSave();

        if (debugLogs)
            Debug.Log("[ProgressManager] Save cleared.");
    }

    public void SaveNow()
    {
        if (_data == null) _data = new SaveData();
        _data.utcTicks = DateTime.UtcNow.Ticks;
        SaveSystem.Save(_data);
    }

    private bool IsAllowedPlayableScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return false;

        if (string.Equals(sceneName, mainMenuSceneName, StringComparison.OrdinalIgnoreCase))
            return false;

        string lower = sceneName.ToLowerInvariant();
        if (blockedKeywords != null)
        {
            foreach (var k in blockedKeywords)
            {
                if (!string.IsNullOrEmpty(k) && lower.Contains(k))
                    return false;
            }
        }

        if (allowedScenePrefixes == null || allowedScenePrefixes.Length == 0) return false;

        foreach (var p in allowedScenePrefixes)
        {
            if (!string.IsNullOrEmpty(p) && sceneName.StartsWith(p, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    private void OnApplicationQuit() => SaveNow();

    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveNow();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus) SaveNow();
    }
}
