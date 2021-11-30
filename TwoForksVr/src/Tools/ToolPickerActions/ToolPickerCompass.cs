using UnityEngine;

namespace TwoForksVr.Tools.ToolPickerActions
{
    public class ToolPickerCompass: ToolPickerAction
    {
        private vgMapController mapController;

        protected override void OnInitialize()
        {
            // TODO do this without FindObjectsOfTypeAll.
            mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
        }

        protected override void OnSelect()
        {
            mapController.OnToggleCompass();
        }

        protected override void OnDeselect()
        {
            mapController.OnToggleCompass();
        }

        protected override bool IsEquipped()
        {
            return mapController.compassEquipped;
        }
    }
}