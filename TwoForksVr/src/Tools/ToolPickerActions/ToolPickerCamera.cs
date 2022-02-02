using UnityEngine;

namespace TwoForksVr.Tools.ToolPickerActions
{
    public class ToolPickerCamera : ToolPickerAction
    {
        private vgPlayerController playerController;

        protected override void OnInitialize()
        {
            playerController = Object.FindObjectOfType<vgPlayerController>();
        }

        protected override void OnEquip()
        {
            playerController.OnCameraToggle();
        }

        protected override void OnUnequip()
        {
            playerController.OnCameraToggle();
            playerController.OnCameraDown();
        }

        protected override bool IsToolEquipped()
        {
            return playerController.cameraActive;
        }

        protected override bool CanEquipTool()
        {
            return playerController.AllowDisposableCameraUse();
        }

        protected override bool IsInitialized()
        {
            return playerController != null;
        }
    }
}