using System;

namespace TwoForksVr.Tools.ToolPickerActions
{
    public abstract class ToolPickerAction
    {
        protected abstract void OnInitialize();
        protected abstract void OnEquip();
        protected abstract void OnUnequip();
        protected abstract bool IsEquipped();
        protected abstract bool CanEquipTool();
        protected abstract bool IsInitialized();

        private void Initialize()
        {
            OnInitialize();
        }

        public void Equip()
        {
            if (!IsInitialized()) Initialize();
            if (IsEquipped()) return;
            OnEquip();
        }

        public void Unequip()
        {
            if (!IsInitialized()) Initialize();
            if (!IsEquipped()) return;
            OnUnequip();
        }

        public bool CanEquip()
        {
            if (!IsInitialized()) Initialize();
            try
            {
                return CanEquipTool();
            }
            catch
            {
                return false;
            }
        }

        public static ToolPickerAction GetToolPickerAction(VrToolItem itemType)
        {
            switch (itemType)
            {
                case VrToolItem.Compass:
                {
                    return new ToolPickerCompass();
                }
                case VrToolItem.Map:
                {
                    return new ToolPickerMap();
                }
                case VrToolItem.Flashlight:
                {
                    return new ToolPickerFlashlight();
                }
                case VrToolItem.DisposableCamera:
                {
                    return new ToolPickerCamera();
                }
                case VrToolItem.Inventory:
                {
                    return new ToolPickerInventory();
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}