using System;

namespace TwoForksVr.Tools.ToolPickerActions
{
    public abstract class ToolPickerAction
    {
        private bool isInitialized;
        protected abstract void OnInitialize();
        protected abstract void OnSelect();
        protected abstract void OnDeselect();
        protected abstract bool IsEquipped();

        private void Initialize()
        {
            OnInitialize();
            isInitialized = true;
        }

        public void Select()
        {
            if (!isInitialized || IsEquipped()) Initialize();
            OnSelect();
        }

        public void Deselect()
        {
            if (!isInitialized || !IsEquipped()) Initialize();
            OnDeselect();
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