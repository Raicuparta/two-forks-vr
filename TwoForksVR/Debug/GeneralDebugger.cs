using System.Linq;
using TwoForksVR.Helpers;
using UnityEngine;

namespace TwoForksVR.Debug
{
    public class GeneralDebugger : MonoBehaviour
    {
        public static Animator PlayerAnimator;

        private void Update()
        {
            UpdateTimeScale();
            UpdateAnimator();
        }

        private static void UpdateTimeScale()
        {
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