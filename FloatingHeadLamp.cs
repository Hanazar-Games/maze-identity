using System.Collections;
using UnityEngine;

public class FloatingHeadLamp : MonoBehaviour
{
    [Header("Timing")]
    public float appearDelay = 2f;

    [Header("Follow Target")]
    public Transform player;                 // 可不填：自动找 tag=Player
    public float heightAbovePlayer = 3f;     // 正上方几米（可调）
    public Vector3 worldOffset = Vector3.zero;
    public bool follow = true;

    [Header("Lamp")]
    public Light lampPrefab;                 // 推荐：Spot Light prefab
    public bool startHidden = true;

    private Light _lamp;
    public Light Lamp => _lamp;

    private void Awake()
    {
        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) player = go.transform;
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnAndAppear());
    }

    private IEnumerator SpawnAndAppear()
    {
        if (lampPrefab != null)
            _lamp = Instantiate(lampPrefab);
        else
            _lamp = new GameObject("PlayerHeadLamp").AddComponent<Light>();

        _lamp.transform.SetParent(null);

        if (startHidden)
            SetLampActive(false);

        if (appearDelay > 0f)
            yield return new WaitForSeconds(appearDelay);

        SetLampActive(true);
        SnapToPlayer();
    }

    private void LateUpdate()
    {
        if (!follow) return;
        if (_lamp == null) return;
        if (!_lamp.gameObject.activeSelf) return;

        SnapToPlayer();
    }

    private void SnapToPlayer()
    {
        if (player == null || _lamp == null) return;
        _lamp.transform.position = player.position + Vector3.up * heightAbovePlayer + worldOffset;
    }

    public void SetLampActive(bool active)
    {
        if (_lamp == null) return;
        _lamp.enabled = active;
        _lamp.gameObject.SetActive(active);
    }

    /// <summary>
    /// 让头灯“传送走”或“禁用”。
    /// disableInstead=true：直接关掉（推荐，最稳）
    /// disableInstead=false：把灯移到很远处（也能达到“场景外”的效果）
    /// </summary>
    public void TeleportLampOutOrDisable(bool disableInstead = true, Vector3 outPosition = default)
    {
        if (_lamp == null) return;

        if (disableInstead)
        {
            SetLampActive(false);
            return;
        }

        if (outPosition == default)
            outPosition = new Vector3(100000f, 100000f, 100000f);

        _lamp.transform.position = outPosition;
    }
}
