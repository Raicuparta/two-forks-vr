using BepInEx.Configuration;
using UnityEngine;

namespace TwoForksVr.Settings;

public static class VrSettings
{
    public enum SmoothRotationSpeedOption
    {
        VerySlow = 1,
        Slow = 2,
        Default = 3,
        Fast = 4,
        VeryFast = 5
    }

    public enum SnapTurnAngleOption
    {
        Angle23 = 23,
        Angle30 = 30,
        Angle45 = 45,
        Angle60 = 60,
        Angle90 = 90
    }

    private const string controlsCategory = "Controls";
    private const string comfortCategory = "Comfort";
    private const string playerBodyCategory = "Player Body";

    public static ConfigFile Config { get; private set; }
    public static ConfigEntry<bool> SnapTurning { get; private set; }
    public static ConfigEntry<bool> ShowLegs { get; private set; }
    public static ConfigEntry<bool> RoomScaleBodyPosition { get; private set; }
    public static ConfigEntry<bool> Teleport { get; private set; }
    public static ConfigEntry<bool> FixedCameraDuringAnimations { get; private set; }
    public static ConfigEntry<bool> LeftHandedMode { get; private set; }
    public static ConfigEntry<bool> SwapSticks { get; private set; }
    public static ConfigEntry<bool> ControllerBasedMovementDirection { get; private set; }
    public static ConfigEntry<SnapTurnAngleOption> SnapTurnAngle { get; private set; }
    public static ConfigEntry<SmoothRotationSpeedOption> SmoothRotationSpeed { get; private set; }

    public static void SetUp(ConfigFile config)
    {
        SetUpResolution();

        Config = config;
        SnapTurning = config.Bind(comfortCategory, "SnapTurning", false,
            "Snap turning|Enabled: snap turning. Disabled: smooth turning.");
        SnapTurnAngle = config.Bind(comfortCategory, "SnapTurnAngle", SnapTurnAngleOption.Angle60,
            "Snap turn angle|How much to turn when snap turning is enabled.");
        SmoothRotationSpeed = config.Bind(comfortCategory, "SmoothRotationSpeed", SmoothRotationSpeedOption.Default,
            "Smooth rotation speed|How fast to turn when snap turning is disabled.");
        Teleport = config.Bind(comfortCategory, "Teleport", false,
            "Fixed camera while moving|\"Teleport\" locomotion. Camera stays still while player moves.");
        FixedCameraDuringAnimations = config.Bind(comfortCategory, "FixedCameraDuringAnimations", false,
            "Fixed camera during animations|Camera stays still during some larger animations.");
        ShowLegs = config.Bind(playerBodyCategory, "ShowLegs", true,
            "Show ghost legs at all times|Helpful for getting a better sense of where the player is standing.");
        RoomScaleBodyPosition = config.Bind(playerBodyCategory, "RoomScaleBodyPosition", true,
            "Make player body follow headset position|Disabling prevents drifting, but you'll need to occasionally recenter manually in the pause menu.");
        ControllerBasedMovementDirection = config.Bind(controlsCategory, "ControllerBasedMovementDirection", false,
            "Controller-based movement direction|Enabled: controller-based direction. Disabled: head-based direction.");
        LeftHandedMode = config.Bind(controlsCategory, "LeftHandedMode", false,
            "Left handed mode|Swaps Henry's hands. Might break some animations.");
        SwapSticks = config.Bind(controlsCategory, "SwapSticks", false,
            "Swap movement / rotation sticks|Swaps controller sticks, independently of handedness.");
    }

    private static void SetUpResolution()
    {
        Screen.SetResolution(1920, 1080, false);
    }
}