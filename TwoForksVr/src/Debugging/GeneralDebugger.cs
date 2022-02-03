using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.Debugging
{
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
            Logs.LogInfo("## Starting key bind logs##");
            foreach (var bind in inputManager.virtualKeyKeyBindMap.Values)
            {
                Logs.LogInfo("bind");
                foreach (var command in bind.commands) Logs.LogInfo($"command: {command.command}");
            }

            foreach (var context in inputManager.contextStack)
            {
                Logs.LogInfo($"## Context {context.name}:");
                foreach (var mapping in context.commandMap)
                {
                    Logs.LogInfo($"# mapping: {mapping.virtualKey}");
                    foreach (var command in mapping.commands) Logs.LogInfo($"command: {command.command}");
                }
            }

            Logs.LogInfo("## Virtual keys: ##");
            foreach (var item in inputManager.customLayout.mapping) Logs.LogInfo($"virtual key: {item.virtualKey}");

            Logs.LogInfo("## Ended key bind logs ##");
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
}