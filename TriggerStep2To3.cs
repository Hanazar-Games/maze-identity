using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class Trigger2To3Start : MonoBehaviour
{
    public Scene2To3SequenceController controller;
    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (controller == null)
        {
            Debug.LogError("[Trigger2To3Start] controller is NULL.");
            return;
        }

        controller.BeginSequence();

        // 可选：触发后让自己失效，防止反复进入
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
    }
}
