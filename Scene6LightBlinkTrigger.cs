using UnityEngine;

public class LightBlinkTrigger : MonoBehaviour
{
    public LightBlinkController blinkController;
    public bool triggerOnce = true;

    private bool _triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered && triggerOnce) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;

        if (blinkController != null)
            blinkController.Activate();
    }

    // 可选：离开 Trigger 停止闪烁
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 如果你想离开就停，打开这一行
        // blinkController.StopBlink();
    }
}
