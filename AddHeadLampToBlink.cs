using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddHeadLampToBlink : MonoBehaviour
{
    public FloatingHeadLamp headLamp;
    public LightBlinkController blinkController;

    [Header("When to add")]
    public float addDelay = 0f; // 通常=0；因为 headLamp 自己会延迟appear

    private void Start()
    {
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        if (addDelay > 0f) yield return new WaitForSeconds(addDelay);

        // 等到 headLamp 真正生成 Light
        while (headLamp != null && headLamp.Lamp == null)
            yield return null;

        if (headLamp == null || headLamp.Lamp == null) yield break;
        if (blinkController == null) yield break;

        AddLight(blinkController, headLamp.Lamp);
    }

    private void AddLight(LightBlinkController controller, Light lamp)
    {
        if (controller.targetLights == null)
        {
            controller.targetLights = new Light[] { lamp };
            return;
        }

        var list = new List<Light>(controller.targetLights.Length + 1);
        for (int i = 0; i < controller.targetLights.Length; i++)
            if (controller.targetLights[i] != null) list.Add(controller.targetLights[i]);

        if (!list.Contains(lamp))
            list.Add(lamp);

        controller.targetLights = list.ToArray();
    }
}
