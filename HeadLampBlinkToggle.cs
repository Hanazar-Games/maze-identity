using System.Collections;
using UnityEngine;

public class HeadLampModeToggle : MonoBehaviour
{
    public enum Mode
    {
        ConstantOn,     // 常亮（不参与 blink）
        BlinkWithScene  // 参与 blink（跟场景灯同频）
    }

    [Header("Refs")]
    public FloatingHeadLamp headLamp;
    public LightBlinkController sceneBlink;

    [Header("Default")]
    public Mode startMode = Mode.ConstantOn;
    public float constantIntensity = 12f;

    [Header("Optional Hotkey")]
    public bool enableHotkey = true;
    public KeyCode toggleKey = KeyCode.B;

    private Mode _mode;

    private void Start()
    {
        _mode = startMode;
        StartCoroutine(WaitLampThenApply());
    }

    private IEnumerator WaitLampThenApply()
    {
        while (headLamp != null && headLamp.Lamp == null)
            yield return null;

        ApplyMode();
    }

    private void Update()
    {
        if (!enableHotkey) return;
        if (Input.GetKeyDown(toggleKey))
            Toggle();
    }

    // UI Button: 常亮
    public void SetConstantOn()
    {
        _mode = Mode.ConstantOn;
        ApplyMode();
    }

    // UI Button: 跟随闪烁
    public void SetBlinkWithScene()
    {
        _mode = Mode.BlinkWithScene;
        ApplyMode();
    }

    // UI Button / Hotkey: 切换
    public void Toggle()
    {
        _mode = (_mode == Mode.ConstantOn) ? Mode.BlinkWithScene : Mode.ConstantOn;
        ApplyMode();
    }

    private void ApplyMode()
    {
        if (headLamp == null || headLamp.Lamp == null) return;
        if (sceneBlink == null) return;

        var lamp = headLamp.Lamp;

        if (_mode == Mode.BlinkWithScene)
        {
            sceneBlink.AddTargetLight(lamp);
        }
        else // ConstantOn
        {
            sceneBlink.RemoveTargetLight(lamp);
            lamp.enabled = true;
            lamp.intensity = constantIntensity;
        }
    }
}
