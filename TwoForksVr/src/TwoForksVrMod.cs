using System.Reflection;
using BepInEx;
using HarmonyLib;
using TwoForksVr.Assets;

namespace TwoForksVr
{
    [BepInPlugin("raicuparta.twoforksvr", "Two Forks VR", "0.0.7")]
    public class TwoForksVrMod : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            VrAssetLoader.LoadAssets();
        }
    }
}
