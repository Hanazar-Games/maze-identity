using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Scene3EndFadeOutTrigger : MonoBehaviour
{
    [Header("Enable")]
    public bool enable = true;

    [Header("Fade Overlay")]
    [Tooltip("CanvasGroup on Canvas/FadeOverlay. Alpha 0 = clear, Alpha 1 = black.")]
    public CanvasGroup overlayGroup;

    [Header("Timing")]
    [Min(0f)] public float delayBeforeFade = 5f;
    [Min(0f)] public float fadeDuration = 3f;

    [Header("Visibility Policy")]
    [Tooltip("Force overlay invisible (alpha=0) whenever this component is enabled / scene starts.")]
    public bool forceInvisibleOnStart = true;

    [Tooltip("Also disable overlay GameObject until triggered (extra safe).")]
    public bool disableOverlayGameObjectUntilTriggered = false;

    [Header("Options")]
    public bool triggerOnce = true;
    public bool useUnscaledTime = true;

    [Tooltip("Require other collider to have tag 'Player'. If your player has no tag, turn this off.")]
    public bool requirePlayerTag = true;

    public bool debugLog = false;

    private bool _fired;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    private void Awake()
    {
        ApplyStartVisibility();
    }

    private void OnEnable()
    {
        ApplyStartVisibility();
    }

    private void ApplyStartVisibility()
    {
        if (!overlayGroup) return;

        if (forceInvisibleOnStart)
            overlayGroup.alpha = 0f;

        if (disableOverlayGameObjectUntilTriggered)
            overlayGroup.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enable) return;
        if (_fired && triggerOnce) return;

        if (requirePlayerTag && !other.CompareTag("Player"))
            return;

        _fired = true;

        if (!overlayGroup)
        {
            Debug.LogError("Scene3EndFadeOutTrigger: Missing overlayGroup.");
            return;
        }

        if (debugLog) Debug.Log("Scene3EndFadeOutTrigger: Triggered.");

        // If we kept it inactive, activate now
        if (disableOverlayGameObjectUntilTriggered && !overlayGroup.gameObject.activeSelf)
            overlayGroup.gameObject.SetActive(true);

        // Ensure it starts transparent at trigger moment
        overlayGroup.alpha = 0f;

        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        if (delayBeforeFade > 0f)
            yield return WaitSeconds(delayBeforeFade);

        if (debugLog) Debug.Log("Scene3EndFadeOutTrigger: Start fading to black.");

        // Fade to black: 0 -> 1
        yield return Fade(overlayGroup, overlayGroup.alpha, 1f, fadeDuration);

        if (debugLog) Debug.Log("Scene3EndFadeOutTrigger: Fade complete.");
    }

    private IEnumerator WaitSeconds(float seconds)
    {
        if (seconds <= 0f) yield break;

        if (useUnscaledTime)
            yield return new WaitForSecondsRealtime(seconds);
        else
            yield return new WaitForSeconds(seconds);
    }

    private IEnumerator Fade(CanvasGroup g, float from, float to, float duration)
    {
        if (!g) yield break;

        g.alpha = from;

        if (duration <= 0f)
        {
            g.alpha = to;
            yield break;
        }

        float t = 0f;
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
