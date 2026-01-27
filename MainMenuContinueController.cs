
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuContinueController : MonoBehaviour
{
    [Header("UI")]
    public Button continueButton;                 // Continue 按钮
    public GameObject continueButtonRoot;         // 可选：整个 Continue 物体（用于隐藏）
    public string fallbackNewGameScene = "Scene1";

    [Header("Optional Fade Loader")]
    public MainMenuAfterPressedButtonFadeInFadeOut fadeLoader; // 你做的 mainmenu 按钮黑场脚本（可不填）

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        bool can = ProgressManager.Instance != null && ProgressManager.Instance.HasProgressToContinue();

        if (continueButton != null) continueButton.interactable = can;
        if (continueButtonRoot != null) continueButtonRoot.SetActive(can);

        Debug.Log($"[MainMenuContinueController] canContinue={can}");
    }

    // 绑定 Continue Button OnClick
    public void OnContinueClicked()
    {
        if (ProgressManager.Instance == null) return;

        var data = ProgressManager.Instance.GetData();
        string target = (data != null) ? data.lastSceneName : "";

        // 安全：如果没有有效进度，直接刷新
        if (!ProgressManager.Instance.HasProgressToContinue())
        {
            Debug.LogWarning("[MainMenuContinueController] Continue clicked but no valid progress.");
            Refresh();
            return;
        }

        LoadSceneWithOptionalFade(target);
    }

    // 绑定 New Game Button OnClick
    public void OnNewGameClicked()
    {
        if (ProgressManager.Instance != null)
            ProgressManager.Instance.ClearSave();

        LoadSceneWithOptionalFade(fallbackNewGameScene);
    }

    // 给 DevTool / 你自己调试用：清除进度
    public void OnClearSaveClicked()
    {
        if (ProgressManager.Instance != null)
            ProgressManager.Instance.ClearSave();

        Refresh();
    }

    private void LoadSceneWithOptionalFade(string sceneName)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"[MainMenuContinueController] Scene '{sceneName}' not in Build Settings.");
            return;
        }

        // 如果你 mainmenu 的“点击按钮先黑场2秒”脚本存在，就走它
        if (fadeLoader != null)
        {
            fadeLoader.FadeThen(() => SceneManager.LoadScene(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
