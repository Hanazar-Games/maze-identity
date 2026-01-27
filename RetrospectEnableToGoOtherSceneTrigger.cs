using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoldGToEnterSceneTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public string playerTag = "Player";

    [Tooltip("需要长按G多少秒才触发")]
    [Min(0.05f)] public float holdSeconds = 1.0f;

    [Header("Target Scene")]
    [Tooltip("手动填写要跳转的场景名（必须在 Build Settings 里）")]
    public string targetSceneName = "Scene1";

    [Header("Input")]
    public KeyCode holdKey = KeyCode.G;

    [Header("Fade Out")]
    [Tooltip("拖入全屏黑幕的 CanvasGroup（alpha 0->1）")]
    public CanvasGroup fadeOverlay;

    [Min(0f)] public float fadeOutSeconds = 0.6f;

    [Header("Hint (Optional)")]
    [Tooltip("拖入 TriggerHintFader：进入显示提示，离开隐藏提示")]
    public TriggerHintFader hintFader;

    [Header("Timing")]
    public bool useUnscaledTime = true;

    [Header("Debug")]
    public bool debugLogs = false;

    private bool _playerInside;
    private float _holdTimer;
    private bool _transitioning;
    private Coroutine _fadeCo;

    private void Awake()
    {
        // 黑幕默认隐藏
        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0f;
            fadeOverlay.interactable = false;
            fadeOverlay.blocksRaycasts = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_transitioning) return;
        if (!other.CompareTag(playerTag)) return;

        _playerInside = true;
        _holdTimer = 0f;

        if (debugLogs) Debug.Log($"[HoldGToEnterSceneTrigger] Player ENTER {name}");

        if (hintFader != null) hintFader.Show();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        _playerInside = false;
        _holdTimer = 0f; // 离开立即失效

        if (debugLogs) Debug.Log($"[HoldGToEnterSceneTrigger] Player EXIT {name} (reset hold)");

        if (hintFader != null) hintFader.Hide();
    }

    private void Update()
    {
        if (_transitioning) return;
        if (!_playerInside) return;

        // 不在触发区域：不计时
        if (!Input.GetKey(holdKey))
        {
            _holdTimer = 0f;
            return;
        }

        _holdTimer += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (debugLogs)
            Debug.Log($"[HoldGToEnterSceneTrigger] Holding {holdKey}: {_holdTimer:F2}/{holdSeconds:F2}");

        if (_holdTimer >= holdSeconds)
        {
            _holdTimer = 0f;
            StartCoroutine(DoTransition());
        }
    }

    private IEnumerator DoTransition()
    {
        if (_transitioning) yield break;
        _transitioning = true;

        if (debugLogs)
            Debug.Log($"[HoldGToEnterSceneTrigger] TRIGGERED -> '{targetSceneName}' (fade {fadeOutSeconds}s)");

        // 触发后立即隐藏提示（可选）
        if (hintFader != null) hintFader.Hide();

        // FadeOut
        if (fadeOverlay != null)
        {
            if (_fadeCo != null) StopCoroutine(_fadeCo);
            _fadeCo = StartCoroutine(FadeCanvasGroup(fadeOverlay, fadeOverlay.alpha, 1f, fadeOutSeconds));
            yield return _fadeCo;
        }
        else
        {
            if (debugLogs) Debug.LogWarning("[HoldGToEnterSceneTrigger] fadeOverlay is NULL, loading without fade.");
        }

        // 安全检查：Build Settings
        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError($"[HoldGToEnterSceneTrigger] Scene '{targetSceneName}' not in Build Settings or name wrong.");
            _transitioning = false;

            // 尝试把黑幕退回去，避免卡黑
            if (fadeOverlay != null)
                StartCoroutine(FadeCanvasGroup(fadeOverlay, fadeOverlay.alpha, 0f, 0.2f));

            yield break;
        }

        SceneManager.LoadScene(targetSceneName);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup g, float from, float to, float duration)
    {
        if (g == null) yield break;

        if (duration <= 0f)
        {
            g.alpha = to;
            yield break;
        }

        float t = 0f;
        g.alpha = from;

        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            g.alpha = Mathf.Lerp(from, to, p);
            yield return null;
        }

        g.alpha = to;
    }
}
