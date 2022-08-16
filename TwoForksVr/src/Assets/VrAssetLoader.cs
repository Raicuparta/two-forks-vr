using System.Collections.Generic;
using System.IO;
using BepInEx;
using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.Assets;

public static class VrAssetLoader
{
    private const string assetsDir = "TwoForksVrAssets/AssetBundles/";
    public static readonly Dictionary<string, Shader> LivShaders = new();
    public static GameObject ToolPickerPrefab { get; private set; }
    public static Shader TMProShader { get; private set; }
    public static Shader FadeShader { get; private set; }
    public static GameObject VrSettingsMenuPrefab { get; private set; }
    public static GameObject LeftHandPrefab { get; private set; }
    public static GameObject RightHandPrefab { get; private set; }
    public static GameObject TeleportTargetPrefab { get; private set; }
    public static Material HenryBodyMaterial { get; private set; }
    public static Material HenryArmsMaterial { get; private set; }
    public static Material HenryBackpackMaterial { get; private set; }

    public static void LoadAssets()
    {
        var bodyBundle = LoadBundle("body");
        LeftHandPrefab = bodyBundle.LoadAsset<GameObject>("left-hand");
        RightHandPrefab = bodyBundle.LoadAsset<GameObject>("right-hand");
        HenryBodyMaterial = bodyBundle.LoadAsset<Material>("HenryBody");
        HenryArmsMaterial = bodyBundle.LoadAsset<Material>("HenryArmsNew");
        HenryBackpackMaterial = bodyBundle.LoadAsset<Material>("HenryBackpack");

        var uiBundle = LoadBundle("ui");
        ToolPickerPrefab = uiBundle.LoadAsset<GameObject>("tool-picker");
        VrSettingsMenuPrefab = uiBundle.LoadAsset<GameObject>("vr-settings-menu");
        TeleportTargetPrefab = uiBundle.LoadAsset<GameObject>("teleport-target");
        TMProShader = uiBundle.LoadAsset<Shader>("TMP_SDF-Mobile");
        FadeShader = uiBundle.LoadAsset<Shader>("SteamVR_Fade");

        var shadersBundle = LoadBundle("liv-shaders");
        foreach (var shader in shadersBundle.LoadAllAssets<Shader>()) LivShaders[shader.name] = shader;
    }

    private static AssetBundle LoadBundle(string assetName)
    {
        var bundle = AssetBundle.LoadFromFile(Path.Combine(Paths.PluginPath, Path.Combine(assetsDir, assetName)));
        if (bundle != null) return bundle;
        Logs.WriteError($"Failed to load AssetBundle {assetName}");
        return null;
    }
}