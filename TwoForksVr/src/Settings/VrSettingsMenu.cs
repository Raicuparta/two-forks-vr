using TwoForksVr.Assets;
using TwoForksVr.Stage;
using TwoForksVr.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TwoForksVr.Settings
{
    public class VrSettingsMenu : MonoBehaviour
    {
        public static VrSettingsMenu Create(VrStage stage)
        {
            var instance = Instantiate(VrAssetLoader.VrSettingsMenuPrefab, stage.transform, false)
                .AddComponent<VrSettingsMenu>();
            instance.gameObject.AddComponent<InteractiveUi>();

            var canvas = instance.GetComponent<Canvas>();
            // High sorting order so it has priority over other menus underneath.
            canvas.sortingOrder = 100;

            var exitButton = instance.transform.Find("ExitButton").GetComponent<Button>();
            exitButton.onClick.AddListener(() => { instance.gameObject.SetActive(false); });

            var layoutGroup = instance.transform.Find("LayoutGroup");

            var toggleObject = layoutGroup.GetComponentInChildren<Toggle>(true).gameObject;
            toggleObject.SetActive(false);

            foreach (var configEntry in VrSettings.Config)
            {
                var toggleInstance = Instantiate(toggleObject, layoutGroup, false);
                toggleInstance.SetActive(true);
                toggleInstance.GetComponentInChildren<Text>().text = configEntry.Value.Description.Description;
                var toggleInput = toggleInstance.GetComponentInChildren<Toggle>();
                toggleInput.isOn = (bool) configEntry.Value.BoxedValue;

                toggleInput.onValueChanged.AddListener(isOn => { configEntry.Value.BoxedValue = isOn; });
            }

            return instance;
        }
    }
}