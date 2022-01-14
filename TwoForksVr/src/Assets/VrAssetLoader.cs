using System.IO;
using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.Assets
{
    public static class VrAssetLoader
    {
        private const string assetsDir = "/BepInEx/plugins/TwoForksVrAssets/AssetBundles/";
        public static GameObject ToolPickerPrefab { get; private set; }
        public static GameObject HandLid { get; private set; }
        public static GameObject ShoeLid { get; private set; }
        public static Texture2D ArmsCutoutTexture { get; private set; }
        public static Texture2D BodyCutoutTexture { get; private set; }
        public static GameObject HandsPrefab { get; private set; }
        public static GameObject VrSettingsMenuPrefab { get; private set; }
        public static GameObject PlayerPrefab { get; private set; }

        public static void LoadAssets()
        {
            var handAsset = LoadBundle("hand");
            var bodyAsset = LoadBundle("body");
            HandLid = handAsset.LoadAsset<GameObject>("HandLid");
            ShoeLid = bodyAsset.LoadAsset<GameObject>("ShoeLid");
            PlayerPrefab = bodyAsset.LoadAsset<GameObject>("Player Prefab");
            ArmsCutoutTexture = handAsset.LoadAsset<Texture2D>("arms-cutout");
            BodyCutoutTexture = bodyAsset.LoadAsset<Texture2D>("body-cutout");
            ToolPickerPrefab = LoadAssetPrefab("tool-picker", "ToolPicker");
            HandsPrefab = LoadAssetPrefab("hands", "VrHands");
            VrSettingsMenuPrefab = LoadAssetPrefab("settings-menu", "VrSettingsMenu");
        }

        private static AssetBundle LoadBundle(string assetName)
        {
            var bundle = AssetBundle.LoadFromFile($"{Directory.GetCurrentDirectory()}{assetsDir}{assetName}");
            if (bundle != null) return bundle;
            Logs.LogError($"Failed to load AssetBundle {assetName}");
            return null;
        }

        private static GameObject LoadAssetPrefab(string assetName, string objectName)
        {
            return LoadBundle(assetName).LoadAsset<GameObject>(objectName);
        }
    }
}