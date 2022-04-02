using System.Reflection;
using BepInEx;
using HarmonyLib;
using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.Settings;
using Valve.VR;

namespace TwoForksVr
{
    [BepInPlugin("raicuparta.twoforksvr", "Two Forks VR", "1.0.0")]
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

            // TODO ask someone to test this on the Steam version of the game.
            var isSteamBuild =
                typeof(vgAchievementManager_Steam).GetMethod(nameof(vgAchievementManager.InitializeService)) != null;

            Logs.LogInfo($"########## IS STEAM BUILD? {isSteamBuild} ##########");

            ApplicationManifestHelper.UpdateManifest(Paths.ManagedPath + @"\..\StreamingAssets\twoforksvr.vrmanifest",
                "steam.app.383870",
                "https://steamcdn-a.akamaihd.net/steam/apps/383870/header.jpg",
                "Firewatch VR",
                "Two Forks VR mod for Firewatch",
                steamBuild: isSteamBuild,
                steamAppId: 383870);
        }
    }
}