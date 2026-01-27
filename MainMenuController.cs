using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Names (must match Build Settings)")]
    public string startSceneName = "Scene1";
    public string optionsSceneName = "Options";

    public void StartGame()
    {
        SceneManager.LoadScene(startSceneName);
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene(optionsSceneName);
    }

    public void QuitGame()
    {
        // 注意：在Unity编辑器里不会真正退出，这是正常的
        Application.Quit();
    }
}
