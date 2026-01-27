using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuActions : MonoBehaviour
{
    [Header("Fader")]
    public ScreenFader fader;

    [Header("Scene Names")]
    public string startSceneName = "Scene1";
    public string optionsSceneName = "Options";

    private void Awake()
    {
        if (fader == null)
            fader = FindObjectOfType<ScreenFader>();
    }

    public void OnStartClicked()
    {
        if (fader == null)
        {
            SceneManager.LoadScene(startSceneName);
            return;
        }

        fader.FadeOutThen(() =>
        {
            SceneManager.LoadScene(startSceneName);
        });
    }

    public void OnOptionsClicked()
    {
        if (fader == null)
        {
            SceneManager.LoadScene(optionsSceneName);
            return;
        }

        fader.FadeOutThen(() =>
        {
            SceneManager.LoadScene(optionsSceneName);
        });
    }

    public void OnQuitClicked()
    {
        if (fader == null)
        {
            UnityEngine.Application.Quit();
            return;
        }

        fader.FadeOutThen(() =>
        {
            UnityEngine.Application.Quit();
        });
    }
}
