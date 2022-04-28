using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.Debugging;

public class GeneralDebugger : MonoBehaviour
{
    private void Update()
    {
        UpdateTimeScale();
        UpdateInputsDebug();
        UpdateSaveActions();
    }

    private static void UpdateInputsDebug()
    {
        var inputManager = vgInputManager.Instance;
        if (!Input.GetKeyDown(KeyCode.Alpha9)) return;
        Logs.WriteInfo("## Starting key bind logs##");
        foreach (var bind in inputManager.virtualKeyKeyBindMap.Values)
        {
            Logs.WriteInfo("bind");
            foreach (var command in bind.commands) Logs.WriteInfo($"command: {command.command}");
        }

        foreach (var context in inputManager.contextStack)
        {
            Logs.WriteInfo($"## Context {context.name}:");
            foreach (var mapping in context.commandMap)
            {
                Logs.WriteInfo($"# mapping: {mapping.virtualKey}");
                foreach (var command in mapping.commands) Logs.WriteInfo($"command: {command.command}");
            }
        }

        Logs.WriteInfo("## Virtual keys: ##");
        foreach (var item in inputManager.customLayout.mapping) Logs.WriteInfo($"virtual key: {item.virtualKey}");

        Logs.WriteInfo("## Ended key bind logs ##");
    }

    private static void UpdateSaveActions()
    {
        if (!Input.GetKeyDown(KeyCode.Alpha1)) return;
        vgSaveManager.Instance.LoadMostRecent();
    }

    private static void UpdateTimeScale()
    {
        if (Input.GetKeyDown(KeyCode.Minus)) Time.timeScale = 0.1f;
        if (Input.GetKeyDown(KeyCode.Equals)) Time.timeScale = Time.timeScale > 1 ? 1 : 10;
    }
}