using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using TwoForksVr.Assets;
using TwoForksVr.Settings;

namespace TwoForksVr
{
    [BepInPlugin("raicuparta.twoforksvr", "Two Forks VR", "0.0.8")]
    public class TwoForksVrMod : BaseUnityPlugin
    {
        private void Awake()
        {
            VrSettings.SetUp(Config);
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            VrAssetLoader.LoadAssets();
        }
    }
}