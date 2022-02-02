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

        protected override void OnEquip()
        {
            flashlightController.ToggleFlashlight();
        }

        protected override void OnUnequip()
        {
            flashlightController.ToggleFlashlight();
        }

        protected override bool IsToolEquipped()
        {
            return flashlightController.isActive;
        }

        protected override bool CanEquipTool()
        {
            return flashlightController.hasFlashlightRequirement.Name != string.Empty &&
                   flashlightController.hasFlashlightRequirement.Verify();
        }

        protected override bool IsInitialized()
        {
            return flashlightController != null;
        }
    }
}