using System.Linq;
using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.Debugging
{
    public class GeneralDebugger : MonoBehaviour
    {
        public static Animator PlayerAnimator;

        private void Update()
        {
            UpdateTimeScale();
            UpdateAnimator();
            UpdateInputsDebug();
            UpdateSaveActions();
        }

        private static void UpdateInputsDebug()
        {
            var inputManager = vgInputManager.Instance;
            if (!UnityEngine.Input.GetKeyDown(KeyCode.Alpha9)) return;
            Logs.LogInfo("## Starting key bind logs##");
            foreach (var bind in inputManager.virtualKeyKeyBindMap.Values)
            {
                Logs.LogInfo($"bind");
                foreach (var command in bind.commands)
                {
                    Logs.LogInfo($"command: {command.command}");
                }
            }
            foreach (var context in inputManager.contextStack)
            {
                Logs.LogInfo($"## Context {context.name}:");
                foreach (var mapping in context.commandMap)
                {
                    Logs.LogInfo($"# mapping: {mapping.virtualKey}");
                    foreach (var command in mapping.commands)
                    {
                        Logs.LogInfo($"command: {command.command}");
                    }
                }
            }
            Logs.LogInfo("## Ended key bind logs ##");
        }

        private static void UpdateSaveActions()
        {
            if (!UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) return;
            vgSaveManager.Instance.LoadMostRecent();
        }

        private static void UpdateTimeScale()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Minus)) Time.timeScale = 0.1f;
            if (UnityEngine.Input.GetKeyDown(KeyCode.Equals)) Time.timeScale = Time.timeScale > 1 ? 1 : 10;
        }

        private static void UpdateAnimator()
        {
            if (!PlayerAnimator || !UnityEngine.Input.GetKeyDown(KeyCode.F11)) return;

            Logs.LogInfo("---- Start animation log ----");
            for (var layerIndex = 0; layerIndex < PlayerAnimator.layerCount; layerIndex++)
            {
                if (PlayerAnimator.GetCurrentAnimatorClipInfoCount(layerIndex) == 0) continue;
                Logs.LogInfo($"Layer Index: {layerIndex.ToString()}");
                Logs.LogInfo($"Layer Name: {PlayerAnimator.GetLayerName(layerIndex)}");
                var animations = PlayerAnimator.GetCurrentAnimatorClipInfo(layerIndex);
                var animationNames =
                    // ReSharper disable once HeapView.ObjectAllocation
                    string.Join(", ", animations.Select(animation => animation.clip.name).ToArray());
                Logs.LogInfo($"Animations [{animationNames}]");
            }

            Logs.LogInfo("---- End animation log ----");
        }
    }
}