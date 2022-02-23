using System;
using TwoForksVr.Settings;
using UnityEngine;

namespace TwoForksVr.Limbs
{
    public class VrHandednessTransformModifier : MonoBehaviour
    {
        private Vector3 leftHandedPosition;
        private Vector3 rightHandedPosition;

        public static void Create(Transform transform, Vector3 rightHandedPosition, Vector3 leftHandedPosition)
        {
            var instance = transform.gameObject.AddComponent<VrHandednessTransformModifier>();
            instance.rightHandedPosition = rightHandedPosition;
            instance.leftHandedPosition = leftHandedPosition;
        }

        private void Awake()
        {
            VrSettings.LeftHandedMode.SettingChanged += LeftHandedModeOnSettingChanged;
        }

        private void Start()
        {
            UpdateTransform();
        }

        private void OnDestroy()
        {
            VrSettings.LeftHandedMode.SettingChanged -= LeftHandedModeOnSettingChanged;
        }

        private void LeftHandedModeOnSettingChanged(object sender, EventArgs e)
        {
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            transform.localPosition = VrSettings.LeftHandedMode.Value ? leftHandedPosition : rightHandedPosition;
        }
    }
}