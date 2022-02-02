namespace TwoForksVr.Tools.ToolPickerActions
{
    public class ToolPickerMap : ToolPickerAction
    {
        private vgMapController mapController;

        protected override void OnInitialize()
        {
            if (!vgPlayerController.playerGameObject) return;
            mapController = vgPlayerController.playerGameObject.GetComponent<vgMapController>();
        }

        protected override void OnEquip()
        {
            mapController.OnToggleMap();
        }

        protected override void OnUnequip()
        {
            mapController.OnToggleMap();
        }

        protected override bool IsToolEquipped()
        {
            return mapController.compassEquipped;
        }

        protected override bool CanEquipTool()
        {
            return mapController.CanUseMap();
        }

        protected override bool IsInitialized()
        {
            return mapController != null;
        }
    }
}