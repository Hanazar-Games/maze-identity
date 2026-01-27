
using UnityEngine;

public class TriggerStartScene3Intro : MonoBehaviour
{
    public Scene3IntroSequenceController controller;

    private bool _fired = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_fired) return;
        if (!other.CompareTag("Player")) return;

        _fired = true;

        if (controller != null) controller.StartSequence();
        else Debug.LogWarning($"[TriggerStartScene3Intro] controller is NULL on {name}");
    }
}
