using UnityEngine;

public class TriggerForwarder : MonoBehaviour
{
    public SceneTransitionController controller;
    public bool isStartTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        UnityEngine.Debug.Log($"[TriggerForwarder] {name} <- Player");

        if (controller == null)
        {
            UnityEngine.Debug.LogError("[TriggerForwarder] controller is NULL (drag it in Inspector)");
            return;
        }

        if (isStartTrigger) controller.OnStartTriggerEnter(other);
        else controller.OnEndTriggerEnter(other);
    }
}
