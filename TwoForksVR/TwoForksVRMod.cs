using System.Reflection;
using BepInEx;
using HarmonyLib;
using TwoForksVR.Assets;

namespace TwoForksVR
{
    [BepInPlugin("raicuparta.twoforksvr", "Two Forks VR", "0.0.6")]
    public class TwoForksVRMod : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            VRAssetLoader.LoadAssets();
        }
    }
}