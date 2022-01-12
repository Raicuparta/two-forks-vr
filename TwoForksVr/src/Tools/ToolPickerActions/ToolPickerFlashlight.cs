using UnityEngine;

namespace TwoForksVr.Tools.ToolPickerActions
{
    public class ToolPickerFlashlight : ToolPickerAction
    {
        private vgFlashlightController flashlightController;

        protected override void OnInitialize()
        {
            flashlightController = Object.FindObjectOfType<vgFlashlightController>();
        }

        protected override void OnSelect()
        {
            flashlightController.ToggleFlashlight();
        }

        protected override void OnDeselect()
        {
            flashlightController.ToggleFlashlight();
        }

        protected override bool IsEquipped()
        {
            return flashlightController.isActive;
        }
    }
}