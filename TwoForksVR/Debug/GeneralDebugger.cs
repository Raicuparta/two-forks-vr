using System.Linq;
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

            TwoForksVRMod.LogInfo("---- Start animation log ----");
            for (var layerIndex = 0; layerIndex < PlayerAnimator.layerCount; layerIndex++)
            {
                if (PlayerAnimator.GetCurrentAnimatorClipInfoCount(layerIndex) == 0) continue;
                TwoForksVRMod.LogInfo($"Layer Index: {layerIndex}");
                TwoForksVRMod.LogInfo($"Layer Name: {PlayerAnimator.GetLayerName(layerIndex)}");
                var animations = PlayerAnimator.GetCurrentAnimatorClipInfo(layerIndex);
                var animationNames =
                    string.Join(", ", animations.Select(animation => animation.clip.name).ToArray());
                TwoForksVRMod.LogInfo($"Animations [{animationNames}]");
            }

            TwoForksVRMod.LogInfo("---- End animation log ----");
        }
    }
}