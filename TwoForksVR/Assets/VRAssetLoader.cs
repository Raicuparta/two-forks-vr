using System.IO;
using UnityEngine;

namespace TwoForksVR.Assets
{
    public static class VRAssetLoader
    {
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
            var myLoadedAssetBundle =
                AssetBundle.LoadFromFile(
                    $"{Directory.GetCurrentDirectory()}/BepInEx/plugins/TwoForksVR/Assets/{assetName}");
            if (myLoadedAssetBundle == null)
            {
                TwoForksVRMod.LogError($"Failed to load AssetBundle {assetName}");
                return null;
            }

            return myLoadedAssetBundle;
        }

        private static GameObject LoadAssetPrefab(string assetName, string objectName)
        {
            return LoadBundle(assetName).LoadAsset<GameObject>(objectName);
        }
    }
}