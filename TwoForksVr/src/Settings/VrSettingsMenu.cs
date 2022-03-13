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

            var scrollView = instance.GetComponentInChildren<ScrollRect>().transform;

            var content = scrollView.Find("Viewport/Content");

            var toggleObject = scrollView.GetComponentInChildren<Toggle>(true).gameObject;
            toggleObject.SetActive(false);

            var sectionObject = content.Find("Section").gameObject;
            sectionObject.SetActive(false);

            var categories = VrSettings.Config.GroupBy(entry => entry.Key.Section);
            categories.ForEach(category =>
            {
                var sectionInstance = Instantiate(sectionObject, content, false);
                sectionInstance.SetActive(true);

                var title = sectionInstance.transform.Find("Title").GetComponent<Text>();
                title.text = category.Key;

                category.ForEach(configEntry =>
                {
                    var toggleInstance = Instantiate(toggleObject, sectionInstance.transform, false);
                    toggleInstance.SetActive(true);

                    var configEntryTextParts = configEntry.Value.Description.Description.Split('|');

                    var label = toggleInstance.transform.Find("Label").GetComponent<Text>();
                    label.text = configEntryTextParts[0];

                    var description = toggleInstance.transform.Find("Description").GetComponent<Text>();
                    if (configEntryTextParts.Length > 1)
                        description.text = configEntryTextParts[1];
                    else
                        Destroy(description.gameObject);

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