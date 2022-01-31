using UnityEngine;

namespace TwoForksVr.Tools.ToolPickerActions
{
    public class ToolPickerInventory : ToolPickerAction
    {
        private vgInventoryController inventoryController;
        private vgInventoryMenuController inventoryMenuController;

        protected override void OnInitialize()
        {
            inventoryController = Object.FindObjectOfType<vgInventoryController>();

            // TODO do this without FindObjectsOfTypeAll.
            inventoryMenuController = Resources.FindObjectsOfTypeAll<vgInventoryMenuController>()[0];
        }

        protected override void OnEquip()
        {
            inventoryController.OnDisplayInventory();
        }

        protected override void OnUnequip()
        {
            inventoryMenuController.OnCloseInventory();
        }

        protected override bool IsEquipped()
        {
            var canvas = inventoryMenuController.canvas;
            return canvas && canvas.gameObject.activeSelf;
        }

        protected override bool CanEquipTool()
        {
            return vgHudManager.Instance != null;
        }

        protected override bool IsInitialized()
        {
            return inventoryController != null && inventoryMenuController != null;
        }
    }
}