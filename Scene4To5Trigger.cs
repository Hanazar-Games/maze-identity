
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Scene4To5Trigger : MonoBehaviour
{
    public Scene4To5SequenceController controller;
    public string playerTag = "Player";

    private Collider _col;
    private bool _fired;

    private void Awake()
    {
        _col = GetComponent<Collider>();
        if (!_col.isTrigger)
            Debug.LogWarning($"[{name}] Collider is not Trigger. Please enable IsTrigger.", this);

        if (controller == null)
            controller = FindObjectOfType<Scene4To5SequenceController>();
    }

    private void Start()
    {
        if (!_fired && IsPlayerInside())
            Fire();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_fired) return;
        if (other.CompareTag(playerTag))
            Fire();
    }

    private bool IsPlayerInside()
    {
        var hits = Physics.OverlapBox(_col.bounds.center, _col.bounds.extents, transform.rotation);
        foreach (var h in hits)
            if (h.CompareTag(playerTag)) return true;
        return false;
    }

    private void Fire()
    {
        _fired = true;

        if (controller == null)
        {
            Debug.LogError($"[Scene4To5Trigger] Controller is NULL on {name}", this);
            return;
        }

        controller.BeginSequence();
    }
}
