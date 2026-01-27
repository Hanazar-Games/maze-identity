
using UnityEngine;

[DisallowMultipleComponent]
public class TriggerStep : MonoBehaviour
{
    [Header("Sequence")]
    public Scene2SequenceController controller;
    [Range(1, 4)]
    public int stepIndex = 1;

    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;

        if (controller != null)
        {
            controller.OnStepTriggered(stepIndex);
        }
        else
        {
            Debug.LogWarning($"[TriggerStep] Controller is NULL on {name}");
        }
    }
}
