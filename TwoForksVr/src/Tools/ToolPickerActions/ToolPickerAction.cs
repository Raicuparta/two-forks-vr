using System;

namespace TwoForksVr.Tools.ToolPickerActions
{
    public abstract class ToolPickerAction
    {
        protected abstract void OnInitialize();
        protected abstract void OnSelect();
        protected abstract void OnDeselect();
        protected abstract bool IsEquipped();
        protected abstract bool IsToolAllowed();
        protected abstract bool IsInitialized();

        private void Initialize()
        {
            OnInitialize();
        }

        public void Select()
        {
            if (!IsInitialized() || IsEquipped()) Initialize();
            OnSelect();
        }

        public void Deselect()
        {
            if (!IsInitialized() || !IsEquipped()) Initialize();
            OnDeselect();
        }

        public bool IsAllowed()
        {
            if (!IsInitialized()) Initialize();
            try
            {
                return IsToolAllowed();
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