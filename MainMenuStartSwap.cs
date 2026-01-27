
using UnityEngine;

public class MainMenuStartSwap : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public RectTransform startButton;    // StartButton 的 RectTransform
    public RectTransform newGameButton;  // New Game 的 RectTransform

    [Header("Behavior")]
    [Tooltip("如果已开始过：NewGame 自动移动到 Start 的位置")]
    public bool moveNewGameToStartPos = true;

    private void Start()
    {
        Apply();
    }

    // 你也可以在回到主菜单时手动调用一次
    public void Apply()
    {
        bool hasStarted = PlayerPrefs.GetInt(MarkStartedAtSceneTransitions.KEY_HAS_STARTED, 0) == 1;

        if (startButton == null || newGameButton == null)
        {
            Debug.LogError("[MainMenuStartSwap] startButton / newGameButton not assigned.");
            return;
        }

        if (!hasStarted)
        {
            // 第一次玩家：显示 Start，隐藏 New Game
            startButton.gameObject.SetActive(true);
            newGameButton.gameObject.SetActive(false);
            return;
        }

        // 已开始过：隐藏 Start，显示 New Game
        startButton.gameObject.SetActive(false);
        newGameButton.gameObject.SetActive(true);

        if (moveNewGameToStartPos)
        {
            // 让 New Game 占用 Start 的位置/大小/缩放（最稳）
            newGameButton.anchoredPosition = startButton.anchoredPosition;
            newGameButton.sizeDelta = startButton.sizeDelta;
            newGameButton.localScale = startButton.localScale;

            // 如果你菜单有用 Anchor，也可以顺带复制
            newGameButton.anchorMin = startButton.anchorMin;
            newGameButton.anchorMax = startButton.anchorMax;
            newGameButton.pivot = startButton.pivot;
        }
    }
}
