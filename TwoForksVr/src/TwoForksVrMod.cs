using System.Reflection;
using BepInEx;
using HarmonyLib;
using TwoForksVr.Assets;
using TwoForksVr.Settings;
using Valve.VR;

namespace TwoForksVr;

[BepInPlugin("raicuparta.twoforksvr", "Two Forks VR", "2.0.0")]
public class TwoForksVrMod : BaseUnityPlugin
{
    private void Awake()
    {
        VrSettings.SetUp(Config);
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        VrAssetLoader.LoadAssets();
        InitSteamVR();
    }

    private static void InitSteamVR()
    {
        SteamVR_Actions.PreInitialize();
        SteamVR.Initialize();
        SteamVR_Settings.instance.pauseGameWhenDashboardVisible = true;
    }
}