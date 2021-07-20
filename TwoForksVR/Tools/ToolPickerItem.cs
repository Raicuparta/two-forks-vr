using System;
using UnityEngine;

namespace TwoForksVR.Tools
{
    public class ToolPickerItem : MonoBehaviour
    {
        private const float circleRadius = 0.25f;

        private VRToolItem itemType;
        private bool isHovered = false;

        public static ToolPickerItem Create(Transform parent, int index)
        {
            var transform = parent.GetChild(index);
            var instance = transform.gameObject.AddComponent<ToolPickerItem>();
            instance.itemType = (VRToolItem)Enum.Parse(typeof(VRToolItem), transform.name);

            float angle = index * Mathf.PI * 2f / parent.childCount;
            instance.transform.localPosition = new Vector3(Mathf.Cos(angle) * circleRadius, Mathf.Sin(angle) * circleRadius, 0);

            instance.SetUpIcon();

            return instance;
        }

        private void SetUpIcon()
        {
            var icon = transform.Find("Icon").GetComponent<SpriteRenderer>();
            icon.material.shader = Shader.Find("Sprites/Default");
        }

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
            switch (itemType)
            {
                case VRToolItem.Compass:
                    {
                        var mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
                        if (!mapController || mapController.compassEquipped) return;

                        mapController.OnToggleCompass();

                        return;
                    }
                case VRToolItem.Map:
                    {
                        var mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
                        if (!mapController || mapController.mapEquipped) return;

                        mapController.OnToggleMap();

                        return;
                    }
                case VRToolItem.Radio:
                    {
                        var radioController = FindObjectOfType<vgPlayerRadioControl>();
                        if (!radioController) return;

                        radioController.OnRadioUp();

                        return;
                    }
                case VRToolItem.Flashlight:
                    {
                        var flashlightController = FindObjectOfType<vgFlashlightController>();
                        if (!flashlightController || flashlightController.isActive) return;

                        flashlightController.ToggleFlashlight();
                        return;
                    }
                case VRToolItem.DisposableCamera:
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
            switch (itemType)
            {
                case VRToolItem.Compass:
                    {
                        var mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
                        if (!mapController || !mapController.compassEquipped) return;

                        mapController.OnToggleCompass();

                        return;
                    }
                case VRToolItem.Map:
                    {
                        var mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
                        if (!mapController || !mapController.mapEquipped) return;

                        mapController.OnToggleMap();

                        return;
                    }
                case VRToolItem.Radio:
                    {
                        var radioController = FindObjectOfType<vgPlayerRadioControl>();
                        if (!radioController) return;

                        radioController.OnRadioDown();
                        return;
                    }
                case VRToolItem.Flashlight:
                    {
                        var flashlightController = FindObjectOfType<vgFlashlightController>();
                        if (!flashlightController || !flashlightController.isActive) return;

                        flashlightController.ToggleFlashlight();
                        return;
                    }
                case VRToolItem.DisposableCamera:
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