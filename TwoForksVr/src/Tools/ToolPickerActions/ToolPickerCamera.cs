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

        protected override void OnSelect()
        {
            playerController.OnCameraToggle();
        }

        protected override void OnDeselect()
        {
            playerController.OnCameraToggle();
            playerController.OnCameraDown();
        }

        protected override bool IsEquipped()
        {
            return playerController.cameraActive;
        }

        protected override bool IsToolAllowed()
        {
            return playerController.AllowDisposableCameraUse();
        }

        protected override bool IsInitialized()
        {
            return playerController != null;
        }
    }
}