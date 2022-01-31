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

        protected override void OnSelect()
        {
            mapController.OnToggleMap();
        }

        protected override void OnDeselect()
        {
            mapController.OnToggleMap();
        }

        protected override bool IsEquipped()
        {
            return mapController.compassEquipped;
        }

        protected override bool IsToolAllowed()
        {
            return mapController.CanUseMap();
        }

        protected override bool IsInitialized()
        {
            return mapController != null;
        }
    }
}