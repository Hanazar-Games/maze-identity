using UnityEngine;

public static class SaveSystem
{
    private const string KEY = "MazeIdentity_SaveData_v1";

    public static void Save(SaveData data)
    {
        if (data == null) return;
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(KEY, json);
        PlayerPrefs.Save();
    }

    public static SaveData Load()
    {
        if (!PlayerPrefs.HasKey(KEY)) return null;
        string json = PlayerPrefs.GetString(KEY, "");
        if (string.IsNullOrEmpty(json)) return null;

        try
        {
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch
        {
            return null;
        }
    }

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(KEY) && !string.IsNullOrEmpty(PlayerPrefs.GetString(KEY, ""));
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(KEY);
        PlayerPrefs.Save();
    }
}
