using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoForksVR.Tools
{
    public class ToolPickerItem : MonoBehaviour
    {
        public ToolPicker.VRToolItem ItemType;

        private bool isHovered = false;

        public void StartHover()
        {
            if (isHovered)
            {
                return;
            }

            isHovered = true;
            transform.localScale *= 1.5f;
        }

        public void EndHover()
        {
            if (!isHovered)
            {
                return;
            }

            isHovered = false;
            transform.localScale = transform.localScale / 1.5f;
        }

        public void Select()
        {
            switch (ItemType)
            {
                case ToolPicker.VRToolItem.Compass:
                    {
                        var mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
                        if (!mapController || mapController.compassEquipped) return;

                        mapController.OnToggleCompass();

                        return;
                    }
                case ToolPicker.VRToolItem.Map:
                    {
                        var mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
                        if (!mapController || mapController.mapEquipped) return;

                        mapController.OnToggleMap();

                        return;
                    }
                case ToolPicker.VRToolItem.Radio:
                    {
                        var radioController = FindObjectOfType<vgPlayerRadioControl>();
                        if (!radioController) return;

                        radioController.OnRadioUp();

                        return;
                    }
                case ToolPicker.VRToolItem.Flashlight:
                    {
                        var flashlightController = FindObjectOfType<vgFlashlightController>();
                        if (!flashlightController || flashlightController.isActive) return;

                        flashlightController.ToggleFlashlight();
                        return;
                    }
                case ToolPicker.VRToolItem.DisposableCamera:
                    {
                        var playerController = FindObjectOfType<vgPlayerController>();
                        if (!playerController || playerController.cameraActive) return;

                        playerController.OnCameraToggle();
                        return;
                    }
            }
        }

        public void Deselect()
        {
            switch (ItemType)
            {
                case ToolPicker.VRToolItem.Compass:
                    {
                        var mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
                        if (!mapController || !mapController.compassEquipped) return;

                        mapController.OnToggleCompass();

                        return;
                    }
                case ToolPicker.VRToolItem.Map:
                    {
                        var mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
                        if (!mapController || !mapController.mapEquipped) return;

                        mapController.OnToggleMap();

                        return;
                    }
                case ToolPicker.VRToolItem.Radio:
                    {
                        var radioController = FindObjectOfType<vgPlayerRadioControl>();
                        if (!radioController) return;

                        radioController.OnRadioDown();
                        return;
                    }
                case ToolPicker.VRToolItem.Flashlight:
                    {
                        var flashlightController = FindObjectOfType<vgFlashlightController>();
                        if (!flashlightController || !flashlightController.isActive) return;

                        flashlightController.ToggleFlashlight();
                        return;
                    }
                case ToolPicker.VRToolItem.DisposableCamera:
                    {
                        var playerController = FindObjectOfType<vgPlayerController>();
                        if (!playerController || !playerController.cameraActive) return;

                        playerController.OnCameraToggle();
                        return;
                    }
            }
        }
    }
}