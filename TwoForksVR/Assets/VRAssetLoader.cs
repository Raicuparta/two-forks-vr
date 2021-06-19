using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Raicuparta.TwoForksVR
{
    public static class VRAssetLoader
    {
        public static GameObject Hand { get; private set; }

        public static void LoadAssets()
        {
            Hand = LoadAssetPrefab("hand", "Hand");
        }

        private static GameObject LoadAssetPrefab(string assetName, string objectName)
        {
            var myLoadedAssetBundle = AssetBundle.LoadFromFile($"{Directory.GetCurrentDirectory()}/Mods/TwoForksVR/{assetName}");
            if (myLoadedAssetBundle == null)
            {
                MelonLogger.Error($"Failed to load AssetBundle {assetName}");
                return null;
            }

            return myLoadedAssetBundle.LoadAsset<GameObject>(objectName);
        }
    }
}
