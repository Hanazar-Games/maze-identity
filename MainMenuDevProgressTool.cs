using System;
using System.Reflection;
using TMPro;
using UnityEngine;

public class MainMenuDevProgressTool : MonoBehaviour
{
    // ===== PlayerPrefs Keys =====
    private const string KEY_LAST_PLAYABLE_SCENE = "MI_LastPlayableScene";
    private const string KEY_HAS_STARTED = "MI_HasStarted";
    private const string KEY_COMPLETED = "MI_Completed";

    [Header("Enable (DEV ONLY)")]
    public bool enableDevTool = false;

    [Header("UI (Optional)")]
    [Tooltip("DevPanel 根物体（你截图里的 DevProgressPanel）。")]
    public GameObject panelRoot;

    [Tooltip("TMP InputField: 输入 Scene 名称，例如 Scene4 / Scene6")]
    public TMP_InputField inputField;

    [Tooltip("可选：提示文本（TMP_Text）")]
    public TMP_Text hintText;

    [Header("Hotkeys")]
    [Tooltip("F6: 仅用于 显示/隐藏 DevPanel（并可选应用输入）。")]
    public KeyCode togglePanelKey = KeyCode.F6;

    [Tooltip("F5: 清理玩家全部进度（DEV ONLY）。")]
    public KeyCode clearProgressKey = KeyCode.F5;

    [Header("Continue Rules")]
    [Tooltip("仅允许这些前缀写入 Continue（Scene1-Scene6）。")]
    public string[] allowedScenePrefixes = new string[] { "Scene1", "Scene2", "Scene3", "Scene4", "Scene5", "Scene6" };

    [Header("Optional: UI Refresh Target")]
    [Tooltip("可选：拖你的 MainMenu Continue 脚本进来（MainMenuContinue / MainMenuContinueController 任意）。不拖也行，会自动找。")]
    public MonoBehaviour mainMenuContinueBehaviour;

    private bool _panelVisible = false;

    private void Awake()
    {
        // 核心要求：MainMenu 默认不显示 DevPanel，只有 F6 才出现
        HidePanelImmediate();
    }

    private void Start()
    {
        SetHint("[DevTool] Hidden. Press F6 to open.");
    }

    private void Update()
    {
        if (!enableDevTool)
        {
            // 如果运行中把 enableDevTool 关了，确保面板也隐藏
            if (_panelVisible) HidePanelImmediate();
            return;
        }

        // F6：呼出/隐藏 DevPanel
        if (Input.GetKeyDown(togglePanelKey))
        {
            TogglePanel();

            // 只有“面板显示状态”下才允许应用输入（避免你说的“其他时间不出现”）
            // 逻辑：打开面板的那一下，如果输入框有 Scene 名称，就直接应用
            if (_panelVisible)
            {
                string s = GetInputSceneName();
                if (!string.IsNullOrEmpty(s))
                {
                    ApplySceneName(s);
                }
                else
                {
                    SetHint("[DevTool] Panel opened. Input scene name then press F6 again (or call ApplyFromInput).");
                }
            }
        }

        // F5：清档（不依赖面板显示）
        if (Input.GetKeyDown(clearProgressKey))
        {
            ClearAllProgress();
        }
    }

    // ===== Public methods (可给按钮用) =====
    public void ApplyFromInput()
    {
        if (!enableDevTool)
        {
            SetHint("[DevTool] Disabled (checkbox off).");
            return;
        }

        if (!_panelVisible)
        {
            SetHint("[DevTool] Panel is hidden. Press F6 to open first.");
            return;
        }

        string sceneName = GetInputSceneName();
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[DevTool] Empty scene name.");
            SetHint("[DevTool] Empty scene name.");
            return;
        }

        ApplySceneName(sceneName);
    }

    public void ClearAllProgress()
    {
        if (!enableDevTool)
        {
            SetHint("[DevTool] Disabled (checkbox off).");
            return;
        }

        // 1) 清理 Continue 三件套
        if (PlayerPrefs.HasKey(KEY_LAST_PLAYABLE_SCENE)) PlayerPrefs.DeleteKey(KEY_LAST_PLAYABLE_SCENE);
        if (PlayerPrefs.HasKey(KEY_HAS_STARTED)) PlayerPrefs.DeleteKey(KEY_HAS_STARTED);
        if (PlayerPrefs.HasKey(KEY_COMPLETED)) PlayerPrefs.DeleteKey(KEY_COMPLETED);
        PlayerPrefs.Save();

        // 2) 同步清理 SaveSystem（如果有 ProgressManager）
        if (ProgressManager.Instance != null)
        {
            // 兼容不同版本：反射调用 ClearSave/SaveNow
            TryInvokeNoArg(ProgressManager.Instance, "ClearSave");
            TryInvokeNoArg(ProgressManager.Instance, "SaveNow");
        }

        // 3) 刷新 UI
        ForceRefreshMainMenuUI();

        Debug.Log("[DevTool] Progress cleared. Continue should be hidden/disabled now.");
        SetHint("[DevTool] Progress cleared.");
    }

    // ===== Panel control =====
    private void TogglePanel()
    {
        if (_panelVisible) HidePanelImmediate();
        else ShowPanelImmediate();
    }

    private void ShowPanelImmediate()
    {
        _panelVisible = true;
        if (panelRoot != null) panelRoot.SetActive(true);

        // 让输入框更好用（可选）
        if (inputField != null)
        {
            inputField.ActivateInputField();
            inputField.Select();
        }
    }

    private void HidePanelImmediate()
    {
        _panelVisible = false;
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    // ===== Core logic =====
    private void ApplySceneName(string sceneName)
    {
        // A) 规则：必须 Scene1-Scene6 前缀
        if (!IsAllowedPrefix(sceneName))
        {
            Debug.LogWarning($"[DevTool] Blocked '{sceneName}' (not allowed prefix).");
            SetHint($"[DevTool] Blocked: {sceneName}");
            return;
        }

        // B) 必须在 Build Settings
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning($"[DevTool] '{sceneName}' not in Build Settings -> not recorded.");
            SetHint($"[DevTool] Not in Build Settings: {sceneName}");
            return;
        }

        // C) 写入 PlayerPrefs Continue
        PlayerPrefs.SetString(KEY_LAST_PLAYABLE_SCENE, sceneName);
        PlayerPrefs.SetInt(KEY_HAS_STARTED, 1);
        PlayerPrefs.SetInt(KEY_COMPLETED, 0); // 强制解除通关隐藏
        PlayerPrefs.Save();

        // D) 同步 ProgressManager（兼容多版本）
        SyncProgressManagerIfPresent(sceneName);

        // E) 刷新 MainMenu Continue UI
        ForceRefreshMainMenuUI();

        Debug.Log($"[DevTool] Continue forced => {sceneName} (ProgressManager synced={ProgressManager.Instance != null})");
        SetHint($"[DevTool] Continue => {sceneName}");
    }

    private void SyncProgressManagerIfPresent(string sceneName)
    {
        if (ProgressManager.Instance == null) return;

        // 优先：如果存在 SetLastScene(string) 就调用
        if (TryInvokeOneString(ProgressManager.Instance, "SetLastScene", sceneName))
        {
            TryInvokeNoArg(ProgressManager.Instance, "SaveNow");
            return;
        }

        // 兜底：如果存在 GetData() 且 data 有 lastSceneName 字段/属性，则写入
        object data = TryInvokeReturnObject(ProgressManager.Instance, "GetData");
        if (data != null)
        {
            if (TrySetMemberString(data, "lastSceneName", sceneName))
            {
                TryInvokeNoArg(ProgressManager.Instance, "SaveNow");
                return;
            }
        }

        Debug.LogWarning("[DevTool] ProgressManager present but cannot sync lastSceneName (no SetLastScene and no GetData().lastSceneName). Using PlayerPrefs only.");
    }

    private bool IsAllowedPrefix(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return false;
        if (allowedScenePrefixes == null || allowedScenePrefixes.Length == 0) return false;

        for (int i = 0; i < allowedScenePrefixes.Length; i++)
        {
            var p = allowedScenePrefixes[i];
            if (!string.IsNullOrEmpty(p) && sceneName.StartsWith(p, StringComparison.Ordinal))
                return true;
        }
        return false;
    }

    private string GetInputSceneName()
    {
        if (inputField == null) return "";
        return (inputField.text ?? "").Trim();
    }

    private void SetHint(string msg)
    {
        if (hintText != null) hintText.text = msg;
    }

    // ===== Refresh MainMenu UI =====
    private void ForceRefreshMainMenuUI()
    {
        MonoBehaviour target = mainMenuContinueBehaviour;

        if (target == null)
        {
            target = FindObjectOfType<MainMenuContinue>(true);
            if (target == null) target = FindObjectOfType<MainMenuContinueController>(true);
        }

        if (target == null)
        {
            Debug.LogWarning("[DevTool] No main menu continue script found. Drag it into 'mainMenuContinueBehaviour'.");
            return;
        }

        if (TryInvokeNoArg(target, "Refresh")) return;
        if (TryInvokeNoArg(target, "RefreshContinueButton")) return;
        if (TryInvokeNoArg(target, "RefreshAll")) return;

        Debug.LogWarning($"[DevTool] Found '{target.GetType().Name}' but no Refresh method matched (Refresh/RefreshContinueButton/RefreshAll).");
    }

    // ===== Reflection helpers =====
    private bool TryInvokeNoArg(object obj, string methodName)
    {
        try
        {
            if (obj == null) return false;
            var mi = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (mi == null) return false;
            if (mi.GetParameters().Length != 0) return false;

            mi.Invoke(obj, null);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool TryInvokeOneString(object obj, string methodName, string arg)
    {
        try
        {
            if (obj == null) return false;
            var mi = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (mi == null) return false;

            var ps = mi.GetParameters();
            if (ps.Length != 1 || ps[0].ParameterType != typeof(string)) return false;

            mi.Invoke(obj, new object[] { arg });
            return true;
        }
        catch
        {
            return false;
        }
    }

    private object TryInvokeReturnObject(object obj, string methodName)
    {
        try
        {
            if (obj == null) return null;
            var mi = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (mi == null) return null;
            if (mi.GetParameters().Length != 0) return null;

            return mi.Invoke(obj, null);
        }
        catch
        {
            return null;
        }
    }

    private bool TrySetMemberString(object target, string memberName, string value)
    {
        try
        {
            if (target == null) return false;
            var t = target.GetType();

            var f = t.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (f != null && f.FieldType == typeof(string))
            {
                f.SetValue(target, value);
                return true;
            }

            var p = t.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (p != null && p.PropertyType == typeof(string) && p.CanWrite)
            {
                p.SetValue(target, value);
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}
