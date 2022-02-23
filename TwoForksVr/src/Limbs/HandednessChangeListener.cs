using System;
using TwoForksVr.Settings;
using UnityEngine;

namespace TwoForksVr.Limbs
{
    public abstract class HandednessChangeListener : MonoBehaviour
    {
        protected void Awake()
        {
            VrSettings.LeftHandedMode.SettingChanged += LeftHandedModeOnSettingChanged;
        }

        protected void Start()
        {
            HandednessChanged();
        }

        protected void OnDestroy()
        {
            VrSettings.LeftHandedMode.SettingChanged -= LeftHandedModeOnSettingChanged;
        }

        private void LeftHandedModeOnSettingChanged(object sender, EventArgs e)
        {
            HandednessChanged();
        }

        protected abstract void HandednessChanged();
    }
}