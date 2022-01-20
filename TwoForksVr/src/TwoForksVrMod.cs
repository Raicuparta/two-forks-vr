using System.Reflection;
using BepInEx;
using HarmonyLib;
using TwoForksVr.Assets;
using TwoForksVr.Settings;
using TwoForksVr.VrInput.Patches;

namespace TwoForksVr
{
    [BepInPlugin("raicuparta.twoforksvr", "Two Forks VR", "0.0.11")]
    public class TwoForksVrMod : BaseUnityPlugin
    {
        private void Awake()
        {
            VrSettings.SetUp(Config);
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            VrAssetLoader.LoadAssets();
            BindingsPatches.Initialize();
        }
    }
}