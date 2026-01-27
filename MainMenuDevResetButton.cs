
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuDevResetButton : MonoBehaviour
{
    [Header("Safety")]
    public bool enableDevReset = true;

    [Tooltip("点击后是否自动重新加载 MainMenu 刷新 UI（Continue/Retrospect 按钮状态）。")]
    public bool reloadMainMenuAfterReset = true;

    [Tooltip("如果你的 ProgressManager 是文件存档，勾上后会额外清除 Continue 存档。")]
    public bool alsoClearProgressManagerSave = true;

    [Header("Optional: Fade")]
    [Tooltip("可选：拖入 ScreenFader；有就 FadeOut 再 reload。")]
    public ScreenFader screenFader;

    [Min(0f)] public float fadeOutSeconds = 0.5f;

    private bool _busy;

    // 绑定到按钮 OnClick
    public void OnDevResetClicked()
    {
        if (!enableDevReset) return;
        if (_busy) return;
        _busy = true;

        // 1) 清 PlayerPrefs（通关解锁/Intro一次性/Options返回标记等）
        MI_ResetAllProgress.ResetAllPlayerPrefs(log: true);

        // 2) 清 ProgressManager 文件存档（Continue）
        if (alsoClearProgressManagerSave && ProgressManager.Instance != null)
        {
            ProgressManager.Instance.ClearSave();
            Debug.Log("[MainMenuDevResetButton] ProgressManager save cleared.");
        }

        // 3) 立刻刷新 UI：最稳妥是 reload MainMenu（确保所有 Awake/Start 重新跑一遍）
        if (!reloadMainMenuAfterReset)
        {
            _busy = false;
            return;
        }

        string current = SceneManager.GetActiveScene().name;

        if (screenFader != null)
        {
            // 如果你的 ScreenFader 支持自定义时间，就用它；否则先临时改成 FadeOutThen
            screenFader.FadeOutThen(() =>
            {
                SceneManager.LoadScene(current);
            });
        }
        else
        {
            SceneManager.LoadScene(current);
        }
    }
}
