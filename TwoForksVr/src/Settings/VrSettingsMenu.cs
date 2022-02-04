using System.Linq;
using TwoForksVr.Assets;
using TwoForksVr.Stage;
using TwoForksVr.UI;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

namespace TwoForksVr.Settings
{
    public class VrSettingsMenu : MonoBehaviour
    {
        public static VrSettingsMenu Create(VrStage stage)
        {
            var instance = Instantiate(VrAssetLoader.VrSettingsMenuPrefab, stage.transform, false)
                .AddComponent<VrSettingsMenu>();

            var canvas = instance.GetComponent<Canvas>();
            // High sorting order so it has priority over other menus underneath.
            canvas.sortingOrder = 100;
            AttachedUi.Create<InteractiveUi>(canvas, stage.GetInteractiveUiTarget());

            var exitButton = instance.transform.Find("ExitButton").GetComponent<Button>();
            exitButton.onClick.AddListener(instance.Close);

            var layoutGroup = instance.transform.Find("LayoutGroup");

            var toggleObject = layoutGroup.GetComponentInChildren<Toggle>(true).gameObject;
            toggleObject.SetActive(false);

            var sectionObject = layoutGroup.Find("Section").gameObject;
            sectionObject.SetActive(false);

            var categories = VrSettings.Config.GroupBy(entry => entry.Key.Section);
            categories.ForEach(category =>
            {
                var sectionInstance = Instantiate(sectionObject, layoutGroup, false);
                sectionInstance.SetActive(true);

                var title = sectionInstance.transform.Find("Title").GetComponent<Text>();
                title.text = category.Key;

                category.ForEach(configEntry =>
                {
                    var toggleInstance = Instantiate(toggleObject, sectionInstance.transform, false);
                    toggleInstance.SetActive(true);
                    toggleInstance.GetComponentInChildren<Text>().text = configEntry.Value.Description.Description;
                    var toggleInput = toggleInstance.GetComponentInChildren<Toggle>();
                    toggleInput.isOn = (bool) configEntry.Value.BoxedValue;

                    toggleInput.onValueChanged.AddListener(isOn => { configEntry.Value.BoxedValue = isOn; });
                });
            });

            instance.Close();

            return instance;
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }
    }
}