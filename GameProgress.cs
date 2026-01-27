
using UnityEngine;

public static class GameProgress
{
    // ✅ 统一使用这个 Key（与你日志里 MI_HasFinishedGame 对齐）
    private const string FINISHED_KEY = "MI_HasFinishedGame";

    // ✅ 兼容旧版本（你之前用过 HasFinishedGame）
    private const string LEGACY_KEY = "HasFinishedGame";

    /// <summary>
    /// 建议打开：能在 Console 明确看到读写与迁移
    /// </summary>
    public static bool DebugLogs = true;

    public static bool HasFinishedGame
    {
        get
        {
            MigrateIfNeeded();

            bool v = PlayerPrefs.GetInt(FINISHED_KEY, 0) == 1;
            if (DebugLogs)
                Debug.Log($"[GameProgress] GET {FINISHED_KEY}={(v ? 1 : 0)}");
            return v;
        }
        set
        {
            // 写入前也做迁移，避免出现“旧key还在但新key没写”的混乱
            MigrateIfNeeded();

            PlayerPrefs.SetInt(FINISHED_KEY, value ? 1 : 0);
            PlayerPrefs.Save();

            if (DebugLogs)
                Debug.Log($"[GameProgress] SET {FINISHED_KEY}={(value ? 1 : 0)}");
        }
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(FINISHED_KEY);
        PlayerPrefs.DeleteKey(LEGACY_KEY);
        PlayerPrefs.Save();

        if (DebugLogs)
            Debug.Log($"[GameProgress] ResetProgress: deleted {FINISHED_KEY} and {LEGACY_KEY}");
    }

    /// <summary>
    /// 如果检测到旧 key 存在，就迁移到新 key，然后删除旧 key。
    /// </summary>
    private static void MigrateIfNeeded()
    {
        // 新 key 已存在：不用迁移
        if (PlayerPrefs.HasKey(FINISHED_KEY))
            return;

        // 旧 key 存在：迁移
        if (PlayerPrefs.HasKey(LEGACY_KEY))
        {
            int oldValue = PlayerPrefs.GetInt(LEGACY_KEY, 0);
            PlayerPrefs.SetInt(FINISHED_KEY, oldValue);
            PlayerPrefs.DeleteKey(LEGACY_KEY);
            PlayerPrefs.Save();

            if (DebugLogs)
                Debug.Log($"[GameProgress] Migrated legacy key '{LEGACY_KEY}'={oldValue} -> '{FINISHED_KEY}'={oldValue}");
        }
    }
}
