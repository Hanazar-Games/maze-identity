using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStateResetter : MonoBehaviour
{
    [Header("Which scenes should be forced UI-safe?")]
    public string[] uiSafeScenes = new string[] { "MainMenu", "Options" };

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var s in uiSafeScenes)
        {
            if (!string.IsNullOrEmpty(s) && scene.name == s)
            {
                ApplyMenuSafeState(true);
                break;
            }
        }
    }

    /// <param name="forMenu">true=主菜单/Options；false=进游戏前可选</param>
    public static void ApplyMenuSafeState(bool forMenu)
    {
        // 你现在说“暂停时间先不搞”，但你之前的代码可能残留 timeScale=0
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (forMenu)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
