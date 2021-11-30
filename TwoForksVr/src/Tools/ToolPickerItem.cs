using System;
using UnityEngine;

namespace TwoForksVr.Tools
{
    public class ToolPickerItem : MonoBehaviour
    {
        private const float circleRadius = 0.25f;
        private bool isHovered;
        private VrToolItem itemType;
        private vgMapController mapController;
        private vgFlashlightController flashlightController;
        private vgPlayerController playerController;
        private vgInventoryController inventoryController;
        private vgInventoryMenuController inventoryMenuController;

        public static ToolPickerItem Create(Transform parent, int index)
        {
            var transform = parent.GetChild(index);
            var instance = transform.gameObject.AddComponent<ToolPickerItem>();
            instance.itemType = (VrToolItem) Enum.Parse(typeof(VrToolItem), transform.name);

            var angle = index * Mathf.PI * 2f / parent.childCount;
            instance.transform.localPosition =
                new Vector3(Mathf.Cos(angle) * circleRadius, Mathf.Sin(angle) * circleRadius, 0);

            // TODO separate into individual classes for each item?
            instance.mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
            instance.flashlightController = FindObjectOfType<vgFlashlightController>();
            instance.playerController = FindObjectOfType<vgPlayerController>();
            instance.inventoryController = FindObjectOfType<vgInventoryController>();
            instance.inventoryMenuController = FindObjectOfType<vgInventoryMenuController>();

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
                    if (!mapController || mapController.compassEquipped) return;

                    mapController.OnToggleCompass();

                    return;
                }
                case VrToolItem.Map:
                {
                    if (!mapController || mapController.mapEquipped) return;

                    mapController.OnToggleMap();

                    return;
                }
                case VrToolItem.Flashlight:
                {
                    if (!flashlightController || flashlightController.isActive) return;

                    flashlightController.ToggleFlashlight();
                    return;
                }
                case VrToolItem.DisposableCamera:
                {
                    if (!playerController || playerController.cameraActive) return;

                    playerController.OnCameraToggle();
                    return;
                }
                case VrToolItem.Inventory:
                {
                    if (!inventoryController) return;

                    inventoryController.OnDisplayInventory();
                    return;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Deselect()
        {
            switch (itemType)
            {
                case VrToolItem.Compass:
                {
                    if (!mapController || !mapController.compassEquipped) return;

                    mapController.OnToggleCompass();

                    return;
                }
                case VrToolItem.Map:
                {
                    if (!mapController || !mapController.mapEquipped) return;

                    mapController.OnToggleMap();

                    return;
                }
                case VrToolItem.Flashlight:
                {
                    if (!flashlightController || !flashlightController.isActive) return;

                    flashlightController.ToggleFlashlight();
                    return;
                }
                case VrToolItem.DisposableCamera:
                {
                    if (!playerController || !playerController.cameraActive) return;

                    playerController.OnCameraToggle();
                    return;
                }
                case VrToolItem.Inventory:
                {
                    if (!inventoryMenuController) return;

                    inventoryMenuController.OnCloseInventory();
                    return;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
