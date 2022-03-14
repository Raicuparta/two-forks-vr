using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
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

            var toggleObject = content.GetComponentInChildren<Toggle>(true).gameObject;
            toggleObject.SetActive(false);

            var dropdownObject = content.GetComponentInChildren<Dropdown>(true).gameObject;
            dropdownObject.SetActive(false);

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
                    if (configEntry.Value.SettingType == typeof(bool))
                        CreateToggle(configEntry, toggleObject, sectionInstance);
                    else if (configEntry.Value.SettingType.IsEnum)
                        CreateDropdown(configEntry, dropdownObject, sectionInstance);
                });
            });

            instance.Close();

            return instance;
        }

        private static void CreateToggle(KeyValuePair<ConfigDefinition, ConfigEntryBase> configEntry,
            GameObject toggleObject,
            GameObject sectionInstance)
        {
            var toggleInstance = Instantiate(toggleObject, sectionInstance.transform, false);
            toggleInstance.SetActive(true);

            SetSettingText(toggleInstance, configEntry);

            var toggleInput = toggleInstance.GetComponentInChildren<Toggle>();
            toggleInput.isOn = (bool) configEntry.Value.BoxedValue;

            toggleInput.onValueChanged.AddListener(isOn => { configEntry.Value.BoxedValue = isOn; });
        }

        private static void CreateDropdown(KeyValuePair<ConfigDefinition, ConfigEntryBase> configEntry,
            GameObject dropdownObject,
            GameObject sectionInstance)
        {
            var dropdownInstance = Instantiate(dropdownObject, sectionInstance.transform, false);
            dropdownInstance.SetActive(true);

            SetSettingText(dropdownInstance, configEntry);

            var dropdownInput = dropdownInstance.GetComponentInChildren<Dropdown>();
            var enumValues = Enum.GetValues(configEntry.Value.SettingType);

            var configEntryTextParts = configEntry.Value.Description.Description.Split('|');

            dropdownInput.options.Clear();
            foreach (var enumValue in enumValues)
                dropdownInput.options.Add(
                    new Dropdown.OptionData($"{configEntryTextParts[0]}: {(int) enumValue}"));

            dropdownInput.value = Array.IndexOf(enumValues, configEntry.Value.BoxedValue);

            dropdownInput.onValueChanged.AddListener(index =>
            {
                configEntry.Value.BoxedValue = enumValues.GetValue(index);
            });
        }

        private static void SetSettingText(GameObject settingInstance,
            KeyValuePair<ConfigDefinition, ConfigEntryBase> configEntry)
        {
            var configEntryTextParts = configEntry.Value.Description.Description.Split('|');

            var label = settingInstance.transform.Find("Label").GetComponent<Text>();
            label.text = configEntryTextParts[0];

            var description = settingInstance.transform.Find("Description").GetComponent<Text>();
            if (configEntryTextParts.Length > 1)
                description.text = configEntryTextParts[1];
            else
                Destroy(description.gameObject);
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