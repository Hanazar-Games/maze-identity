using UnityEngine;

public class DisableLightBlinkAtBrightTrigger : MonoBehaviour
{
    public LightBlinkController blinkController;

    public bool triggerOnce = true;
    public string requiredTag = "Player";

    [Header("Behavior")]
    public bool disableBlinkScriptAfterStop = true;
    public bool keepLightBrightAfterStop = true;

    [Header("Head Lamp")]
    public FloatingHeadLamp headLamp;
    public bool disableLampInsteadOfTeleport = true;
    public Vector3 teleportOutPosition = new Vector3(100000f, 100000f, 100000f);

    private bool _triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered && triggerOnce) return;
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return;

        _triggered = true;

        if (blinkController != null)
            blinkController.StopAtBrightThenDisable(disableBlinkScriptAfterStop, keepLightBrightAfterStop);

        // 头灯“传送走”或 disable
        if (headLamp != null)
            headLamp.TeleportLampOutOrDisable(disableLampInsteadOfTeleport, teleportOutPosition);
    }
}
