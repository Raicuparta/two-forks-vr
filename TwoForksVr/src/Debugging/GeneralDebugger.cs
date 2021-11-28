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
            if (!UnityEngine.Input.GetKeyDown(KeyCode.Minus)) return;
            Logs.LogInfo("## Starting key bind logs##");
            var inputManager = FindObjectOfType<vgInputManager>();
            foreach (var bind in inputManager.virtualKeyKeyBindMap.Values.Where(bind => bind.commands.Count != 0))
            {
                Logs.LogInfo($"{bind.commands[0].command}");
                for (var i = 0; i < bind.keyData.names.Count; i++)
                {
                    bind.keyData.names[i] = bind.commands[0].command;
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
                Logs.LogInfo($"Layer Index: {layerIndex}");
                Logs.LogInfo($"Layer Name: {PlayerAnimator.GetLayerName(layerIndex)}");
                var animations = PlayerAnimator.GetCurrentAnimatorClipInfo(layerIndex);
                var animationNames =
                    string.Join(", ", animations.Select(animation => animation.clip.name).ToArray());
                Logs.LogInfo($"Animations [{animationNames}]");
            }

            Logs.LogInfo("---- End animation log ----");
        }
    }
}