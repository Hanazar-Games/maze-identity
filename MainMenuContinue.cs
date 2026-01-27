
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuContinue : MonoBehaviour
{
    [Header("UI")]
    public Button continueButton;

    [Tooltip("如果你希望“没有存档时直接隐藏Continue”，把这里拖成 Continue 按钮的根物体（通常就是按钮本体 GameObject）。不填则仅设置 interactable。")]
    public GameObject continueButtonRoot;

    [Header("New Game")]
    public string fallbackNewGameScene = "Scene1"; // 没存档时 New Game 去哪

    [Header("Continue Rules")]
    [Tooltip("只允许 Continue 的场景前缀（默认 Scene1~Scene6）。")]
    public string[] allowedScenePrefixes = new string[] { "Scene1", "Scene2", "Scene3", "Scene4", "Scene5", "Scene6" };

    [Tooltip("自动清理非法/脏的 lastSceneName（例如被写成 Options/Transition/MainMenu/End）。")]
    public bool autoFixInvalidContinue = true;

    [Header("Optional Fade Loader")]
    [Tooltip("可选：如果你的主菜单有统一的黑场淡出加载器，把它拖进来（它需要提供一个公共方法 LoadSceneWithFade(string)）。不填则直接 LoadScene。")]
    public MonoBehaviour fadeLoader;

    private System.Reflection.MethodInfo _fadeLoadMethod;

    private void Awake()
    {
        // 缓存 fadeLoader 的反射方法（避免你项目里类名不一致导致编译失败）
        if (fadeLoader != null)
        {
            _fadeLoadMethod = fadeLoader.GetType().GetMethod(
                "LoadSceneWithFade",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                null,
                new System.Type[] { typeof(string) },
                null
            );

            if (_fadeLoadMethod == null)
            {
                Debug.LogWarning("[MainMenuContinue] fadeLoader assigned but no public method LoadSceneWithFade(string) found. Will fallback to direct LoadScene.");
            }
        }
    }

    private void Start()
    {
        // 确保 ProgressManager 存在（DontDestroyOnLoad 的 singleton 一般就是它）
        if (ProgressManager.Instance == null)
        {
            Debug.LogWarning("[MainMenuContinue] ProgressManager not found in scene.");
        }

        RefreshContinueButton();
    }

    public void RefreshContinueButton()
    {
        string target = GetContinueTargetSafe(out bool hasProgress, out bool valid);

        // 显示策略：必须“有进度且有效”才显示/可点
        bool show = hasProgress && valid;

        if (continueButtonRoot != null)
            continueButtonRoot.SetActive(show);

        if (continueButton != null)
            continueButton.interactable = show;

        Debug.Log($"[MainMenuContinue] target='{target}', hasProgress={hasProgress}, valid={valid}, show={show}");
    }

    public void OnContinueClicked()
    {
        string target = GetContinueTargetSafe(out bool hasProgress, out bool valid);

        if (!hasProgress || !valid)
        {
            Debug.LogWarning($"[MainMenuContinue] Continue blocked. target='{target}', hasProgress={hasProgress}, valid={valid}");
            RefreshContinueButton();
            return;
        }

        LoadSceneSmart(target);
    }

    public void OnNewGameClicked()
    {
        // 新开档：清除旧进度，然后进第一关
        if (ProgressManager.Instance != null)
            ProgressManager.Instance.ClearSave();

        LoadSceneSmart(fallbackNewGameScene);
    }

    public void OnClearSaveClicked()
    {
        if (ProgressManager.Instance != null)
            ProgressManager.Instance.ClearSave();

        RefreshContinueButton();
    }

    // -------------------------
    // Internal helpers
    // -------------------------

    private string GetContinueTargetSafe(out bool hasProgress, out bool valid)
    {
        hasProgress = false;
        valid = false;

        if (ProgressManager.Instance == null)
            return "";

        hasProgress = ProgressManager.Instance.HasProgressToContinue();
        if (!hasProgress)
            return "";

        var data = ProgressManager.Instance.GetData();
        if (data == null || string.IsNullOrWhiteSpace(data.lastSceneName))
        {
            if (autoFixInvalidContinue) ProgressManager.Instance.ClearSave();
            hasProgress = false;
            return "";
        }

        string target = data.lastSceneName.Trim();
        valid = IsAllowedContinueScene(target);

        if (!valid && autoFixInvalidContinue)
        {
            Debug.LogWarning($"[MainMenuContinue] Invalid continue target '{target}', clearing save.");
            ProgressManager.Instance.ClearSave();
            hasProgress = false;
            return "";
        }

        return target;
    }

    private bool IsAllowedContinueScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return false;

        // 1) 必须匹配允许前缀（Scene1~6）
        bool okPrefix = false;
        if (allowedScenePrefixes != null)
        {
            foreach (var p in allowedScenePrefixes)
            {
                if (!string.IsNullOrEmpty(p) && sceneName.StartsWith(p))
                {
                    okPrefix = true;
                    break;
                }
            }
        }
        if (!okPrefix) return false;

        // 2) 强排除：Options/MainMenu/Transition/End（不区分大小写）
        string lower = sceneName.ToLowerInvariant();
        if (lower.Contains("options")) return false;
        if (lower.Contains("mainmenu")) return false;
        if (lower.Contains("transition")) return false;
        if (lower.Contains("end")) return false;

        return true;
    }

    private void LoadSceneSmart(string sceneName)
    {
        // 可选：走 fadeLoader 的 LoadSceneWithFade(string)
        if (fadeLoader != null && _fadeLoadMethod != null)
        {
            _fadeLoadMethod.Invoke(fadeLoader, new object[] { sceneName });
            return;
        }

        // 否则直接加载
        SceneManager.LoadScene(sceneName);
    }
}
