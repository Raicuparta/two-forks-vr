using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using TwoForksVr.Assets;

namespace TwoForksVr
{
    [BepInPlugin("raicuparta.twoforksvr", "Two Forks VR", "0.0.8")]
    public class TwoForksVrMod : BaseUnityPlugin
    {
        private ConfigEntry<bool> handOrientedMovement;
        private ConfigEntry<bool> snapTurning;

        private void Awake()
        {
            SetUpConfig();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            VrAssetLoader.LoadAssets();
        }
        
        private void SetUpConfig()
        {
            handOrientedMovement = Config.Bind("Config", "HandOrientedMovement", false,
                "True: hand oriented movement. False: head oriented movement.");
            snapTurning = Config.Bind("Config", "SnapTurning", false,
                "True: snap turning. False: smooth turning.");
        }
    }
}