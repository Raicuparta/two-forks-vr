namespace TwoForksVr.Tools.ToolPickerActions;

public class ToolPickerCompass : ToolPickerAction
{
    private vgMapController mapController;

    protected override void OnInitialize()
    {
        if (!vgPlayerController.playerGameObject) return;
        mapController = vgPlayerController.playerGameObject.GetComponent<vgMapController>();
    }

    protected override void OnEquip()
    {
        mapController.OnToggleCompass();
    }

    protected override void OnUnequip()
    {
        mapController.OnToggleCompass();
    }

    protected override bool IsToolEquipped()
    {
        return mapController.compassEquipped;
    }

    protected override bool CanEquipTool()
    {
        return mapController.CanUseCompass();
    }

    protected override bool IsInitialized()
    {
        return mapController != null;
    }
}