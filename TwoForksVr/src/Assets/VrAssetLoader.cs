using System.IO;
using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.Assets
{
    public static class VrAssetLoader
    {
        private const string assetsDir = "/BepInEx/plugins/TwoForksVrAssets/AssetBundles/";
        public static GameObject ToolPickerPrefab { get; private set; }
        public static GameObject ShoeLid { get; private set; }
        public static Texture2D BodyCutoutTexture { get; private set; }
        public static GameObject VrSettingsMenuPrefab { get; private set; }
        public static GameObject LeftHandPrefab { get; private set; }
        public static GameObject RightHandPrefab { get; private set; }
        public static GameObject FadeOverlayPrefab { get; private set; }

        public static void LoadAssets()
        {
            var bodyAsset = LoadBundle("body");
            ShoeLid = bodyAsset.LoadAsset<GameObject>("ShoeLid");
            LeftHandPrefab = bodyAsset.LoadAsset<GameObject>("left-hand");
            RightHandPrefab = bodyAsset.LoadAsset<GameObject>("right-hand");
            BodyCutoutTexture = bodyAsset.LoadAsset<Texture2D>("body-cutout");
            ToolPickerPrefab = LoadAssetPrefab("tool-picker", "ToolPicker");
            VrSettingsMenuPrefab = LoadAssetPrefab("settings-menu", "VrSettingsMenu");
            FadeOverlayPrefab = LoadAssetPrefab("camera", "FadeOverlay");
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