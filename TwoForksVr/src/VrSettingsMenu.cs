using TwoForksVr.Assets;
using TwoForksVr.Stage;
using TwoForksVr.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TwoForksVr
{
    public class VrSettingsMenu: MonoBehaviour
    {
        public static VrSettingsMenu Create(VrStage stage)
        {
            var instance = Instantiate(VrAssetLoader.VrSettingsMenuPrefab, stage.transform, false).AddComponent<VrSettingsMenu>();
            instance.gameObject.AddComponent<InteractiveUi>();

            var canvas = instance.GetComponent<Canvas>();
            canvas.sortingOrder = 1;

            var layoutGroup = instance.transform.Find("LayoutGroup");
            var firstSelectable = layoutGroup.gameObject.GetComponentInChildren<Selectable>();
            firstSelectable.Select();

            var toggleObject = layoutGroup.GetComponentInChildren<Toggle>().gameObject;

            foreach (var configEntry in TwoForksVrMod.ModConfig)
            {
                var toggleInstance = Instantiate(toggleObject, layoutGroup, false);
                toggleInstance.GetComponentInChildren<Text>().text = configEntry.Value.Description.Description;
                var toggleInput = toggleInstance.GetComponentInChildren<Toggle>();
                
                toggleInput.onValueChanged.AddListener((value) =>
                {
                    configEntry.Value.SetSerializedValue(value.ToString());
                });
                
            }

            return instance;
        }
    }
}