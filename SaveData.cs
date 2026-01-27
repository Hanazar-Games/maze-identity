using System;

[Serializable]
public class SaveData
{
    public string lastSceneName = "";        // Continue 目标（只允许 Scene1-6）
    public string lastCheckpointId = "";     // 可选：未来做更细 checkpoint 用
    public long utcTicks = 0;

    // MainMenu UI 逻辑用
    public bool hasStarted = false;

    // 通关逻辑：true 时 Continue 必须隐藏
    public bool completed = false;
}
