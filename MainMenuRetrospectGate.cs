
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuRetrospectGate : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Retrospect 按钮的根物体（建议拖层级里的 Retrospectmode）")]
    public GameObject retrospectButtonRoot;

    [Header("Scene")]
    public string retrospectSceneName = "RetrospectModeScene";

    [Header("Fade (Use existing ScreenFader)")]
    [Tooltip("拖入你的 ScreenFader_GO 上的 ScreenFader 组件")]
    public ScreenFader screenFader;

    [Header("Debug Hotkeys (remove before release)")]
    public bool enableDebugHotkeys = true;
    public KeyCode unlockKey = KeyCode.F8; // 强制解锁（模拟通关）
    public KeyCode resetKey = KeyCode.F7;  // 清除通关记录

    private bool _isTransitioning;

    private void Awake()
    {
        RefreshRetrospectButton();
    }

    private void Start()
    {
        RefreshRetrospectButton();
    }

    private void RefreshRetrospectButton()
    {
        if (retrospectButtonRoot == null)
        {
            Debug.LogWarning("[MainMenuRetrospectGate] retrospectButtonRoot is NULL.");
            return;
        }

        bool finished = GameProgress.HasFinishedGame;

        retrospectButtonRoot.SetActive(finished);

        Debug.Log($"[MainMenuRetrospectGate] Refresh: HasFinishedGame={finished}, " +
                  $"buttonActive={retrospectButtonRoot.activeSelf}");
    }


    // ✅ Button OnClick 绑定这个
    public void OnRetrospectClicked()
    {
        EnterRetrospectMode();
    }

    public void EnterRetrospectMode()
    {
        if (_isTransitioning) return;

        if (!GameProgress.HasFinishedGame)
        {
            Debug.LogWarning("[MainMenu] RetrospectMode locked: game not finished.");
            RefreshRetrospectButton();
            return;
        }

        if (string.IsNullOrWhiteSpace(retrospectSceneName))
        {
            Debug.LogError("[MainMenu] retrospectSceneName is empty.");
            return;
        }

        _isTransitioning = true;

        // 关键：统一交给 ScreenFader 做 FadeOut，避免你之前那个脚本的 overlay 冲突
        if (screenFader != null)
        {
            screenFader.FadeOutThen(() =>
            {
                SceneManager.LoadScene(retrospectSceneName);
            });
        }
        else
        {
            // 没绑定 fader 就直接切（至少不崩）
            Debug.LogWarning("[MainMenuRetrospectGate] screenFader is NULL, loading directly.");
            SceneManager.LoadScene(retrospectSceneName);
        }
    }

    private void Update()
    {
        if (!enableDebugHotkeys) return;

        // F8：强制解锁（测试用）
        if (Input.GetKeyDown(unlockKey))
        {
            GameProgress.HasFinishedGame = true;
            Debug.Log("[DEBUG] Retrospect unlocked (F8).");
            RefreshRetrospectButton();
        }

        // F7：清除通关记录（回到第一次进入）
        if (Input.GetKeyDown(resetKey))
        {
            GameProgress.ResetProgress();
            Debug.Log("[DEBUG] Retrospect progress reset (F7).");
            RefreshRetrospectButton();
        }
    }
}
