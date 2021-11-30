using UnityEngine;

namespace TwoForksVr.Tools.ToolPickerActions
{
    public class ToolPickerMap: ToolPickerAction
    {
        private vgMapController mapController;

        protected override void OnInitialize()
        {
            // TODO do this without FindObjectsOfTypeAll.
            mapController = Resources.FindObjectsOfTypeAll<vgMapController>()[0];
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
    }
}