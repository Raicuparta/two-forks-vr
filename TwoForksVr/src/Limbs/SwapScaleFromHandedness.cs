using TwoForksVr.Settings;
using UnityEngine;

namespace TwoForksVr.Limbs
{
    public class SwapScaleFromHandedness : HandednessChangeListener
    {
        protected Vector3 leftHandedScale;
        protected Vector3 rightHandedScale;

        public static void Create(Transform transform, Vector3 rightHandedScale, Vector3 leftHandedScale)
        {
            var instance = transform.gameObject.AddComponent<SwapScaleFromHandedness>();
            instance.rightHandedScale = rightHandedScale;
            instance.leftHandedScale = leftHandedScale;
        }

        protected override void HandednessChanged()
        {
            transform.localScale = VrSettings.LeftHandedMode.Value ? leftHandedScale : rightHandedScale;
        }
    }
}