using System.IO;
using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.Assets
{
    public static class VRAssetLoader
    {
        private const string assetsDir = "/BepInEx/plugins/TwoForksVrAssets/AssetBundles/";
        public static GameObject ToolPicker { get; private set; }
        public static GameObject HandLid { get; private set; }
        public static Texture2D ArmsCutoutTexture { get; private set; }
        public static GameObject Hands { get; private set; }

        public static void LoadAssets()
        {
            var handAsset = LoadBundle("hand");
            HandLid = handAsset.LoadAsset<GameObject>("HandLid");
            ArmsCutoutTexture = handAsset.LoadAsset<Texture2D>("arms-cutout");
            ToolPicker = LoadAssetPrefab("tool-picker", "ToolPicker");
            Hands = LoadAssetPrefab("hands", "VRHands");
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
