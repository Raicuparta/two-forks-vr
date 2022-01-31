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
        public static Shader TMProShader { get; private set; }
        public static GameObject VrSettingsMenuPrefab { get; private set; }
        public static GameObject LeftHandPrefab { get; private set; }
        public static GameObject RightHandPrefab { get; private set; }
        public static GameObject FadeOverlayPrefab { get; private set; }
        public static GameObject TeleportTargetPrefab { get; private set; }

        public static void LoadAssets()
        {
            var bodyBundle = LoadBundle("body");
            ShoeLid = bodyBundle.LoadAsset<GameObject>("shoe-lid");
            LeftHandPrefab = bodyBundle.LoadAsset<GameObject>("left-hand");
            RightHandPrefab = bodyBundle.LoadAsset<GameObject>("right-hand");
            BodyCutoutTexture = bodyBundle.LoadAsset<Texture2D>("body-cutout");

            var uiBundle = LoadBundle("ui");
            ToolPickerPrefab = uiBundle.LoadAsset<GameObject>("tool-picker");
            VrSettingsMenuPrefab = uiBundle.LoadAsset<GameObject>("vr-settings-menu");
            FadeOverlayPrefab = uiBundle.LoadAsset<GameObject>("fade-overlay");
            TeleportTargetPrefab = uiBundle.LoadAsset<GameObject>("teleport-target");
            TMProShader = uiBundle.LoadAsset<Shader>("TMP_SDF-Mobile");
            Logs.LogInfo($"### TMProShader?? {TMProShader}");
        }

        private static AssetBundle LoadBundle(string assetName)
        {
            var bundle = AssetBundle.LoadFromFile($"{Directory.GetCurrentDirectory()}{assetsDir}{assetName}");
            if (bundle != null) return bundle;
            Logs.LogError($"Failed to load AssetBundle {assetName}");
            return null;
        }
    }
}