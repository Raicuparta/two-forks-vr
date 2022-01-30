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

        protected override void OnSelect()
        {
            inventoryController.OnDisplayInventory();
        }

        protected override void OnDeselect()
        {
            inventoryMenuController.OnCloseInventory();
        }

        protected override bool IsEquipped()
        {
            var canvas = inventoryMenuController.canvas;
            return canvas && canvas.gameObject.activeSelf;
        }

        protected override bool IsToolAllowed()
        {
            return vgHudManager.Instance != null;
        }
    }
}