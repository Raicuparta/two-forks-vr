using System;
using UnityEngine;

namespace TwoForksVr.Tools
{
    public class ToolPickerItem : MonoBehaviour
    {
        private const float circleRadius = 0.25f;
        private bool isHovered;

        private VrToolItem itemType;

        public static ToolPickerItem Create(Transform parent, int index)
        {
            var transform = parent.GetChild(index);
            var instance = transform.gameObject.AddComponent<ToolPickerItem>();
            instance.itemType = (VrToolItem) Enum.Parse(typeof(VrToolItem), transform.name);

            var angle = index * Mathf.PI * 2f / parent.childCount;
            instance.transform.localPosition =
                new Vector3(Mathf.Cos(angle) * circleRadius, Mathf.Sin(angle) * circleRadius, 0);

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
            if (isHovered) return;

            isHovered = true;
            transform.localScale *= 1.5f;
        }

        public void EndHover()
        {
            if (!isHovered) return;

            isHovered = false;
            transform.localScale /= 1.5f;
        }

        public void Select()
        {
            switch (itemType)
            {
                case VrToolItem.Compass:
                {
                    var mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
                    if (!mapController || mapController.compassEquipped) return;

                    mapController.OnToggleCompass();

                    return;
                }
                case VrToolItem.Map:
                {
                    var mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
                    if (!mapController || mapController.mapEquipped) return;

                    mapController.OnToggleMap();

                    return;
                }
                case VrToolItem.Flashlight:
                {
                    var flashlightController = FindObjectOfType<vgFlashlightController>();
                    if (!flashlightController || flashlightController.isActive) return;

                    flashlightController.ToggleFlashlight();
                    return;
                }
                case VrToolItem.DisposableCamera:
                {
                    var playerController = FindObjectOfType<vgPlayerController>();
                    if (!playerController || playerController.cameraActive) return;

                    playerController.OnCameraToggle();
                    return;
                }
                case VrToolItem.Inventory:
                {
                    var inventoryController = FindObjectOfType<vgInventoryController>();
                    inventoryController.OnDisplayInventory();
                    return;
                }
            }
        }

        public void Deselect()
        {
            switch (itemType)
            {
                case VrToolItem.Compass:
                {
                    var mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
                    if (!mapController || !mapController.compassEquipped) return;

                    mapController.OnToggleCompass();

                    return;
                }
                case VrToolItem.Map:
                {
                    var mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
                    if (!mapController || !mapController.mapEquipped) return;

                    mapController.OnToggleMap();

                    return;
                }
                case VrToolItem.Flashlight:
                {
                    var flashlightController = FindObjectOfType<vgFlashlightController>();
                    if (!flashlightController || !flashlightController.isActive) return;

                    flashlightController.ToggleFlashlight();
                    return;
                }
                case VrToolItem.DisposableCamera:
                {
                    var playerController = FindObjectOfType<vgPlayerController>();
                    if (!playerController || !playerController.cameraActive) return;

                    playerController.OnCameraToggle();
                    return;
                }
                case VrToolItem.Inventory:
                {
                    var inventoryMenuController = FindObjectOfType<vgInventoryMenuController>();
                    if (!inventoryMenuController) return;

                    inventoryMenuController.OnCloseInventory();
                    return;
                }
            }
        }
    }
}
