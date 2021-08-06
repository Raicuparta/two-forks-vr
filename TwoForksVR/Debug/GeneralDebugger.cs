using MelonLoader;
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

        private void UpdateTimeScale()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Equals))
            {
                Time.timeScale = Time.timeScale > 1 ? 1 : 10;
            }
        }

        private void UpdateAnimator()
        {
            if (PlayerAnimator == null)
            {
                return;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.F11))
            {
                if (!PlayerAnimator)
                {
                    return;
                }
                MelonLogger.Msg("---- Start animation log ----");
                for (int layerIndex = 0; layerIndex < PlayerAnimator.layerCount; layerIndex++)
                {
                    if (PlayerAnimator.GetCurrentAnimatorClipInfoCount(layerIndex) == 0)
                    {
                        continue;
                    }
                    MelonLogger.Msg($"Layer Index: {layerIndex}");
                    MelonLogger.Msg($"Layer Name: {PlayerAnimator.GetLayerName(layerIndex)}");
                    var animations = PlayerAnimator.GetCurrentAnimatorClipInfo(layerIndex);
                    var animationNames = string.Join(", ", animations.Select(animation => animation.clip.name).ToArray());
                    MelonLogger.Msg($"Animations [{animationNames}]");
                }
                MelonLogger.Msg("---- End animation log ----");
            }
        }
    }
}
