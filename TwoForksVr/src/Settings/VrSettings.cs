using BepInEx.Configuration;
using UnityEngine;

namespace TwoForksVr.Settings
{
    public static class VrSettings
    {
        private const string controlsCategory = "Controls";
        private const string comfortCategory = "Comfort";
        private const string playerBodyCategory = "Player Body";

        public static ConfigFile Config { get; private set; }
        public static ConfigEntry<bool> SnapTurning { get; private set; }
        public static ConfigEntry<bool> ShowLegs { get; private set; }
        public static ConfigEntry<bool> Teleport { get; private set; }
        public static ConfigEntry<bool> FixedCameraDuringAnimations { get; private set; }
        public static ConfigEntry<bool> LeftHandedMode { get; private set; }
        public static ConfigEntry<bool> SwapSticks { get; private set; }
        public static ConfigEntry<bool> ControllerBasedMovementDirection { get; private set; }

        public static void SetUp(ConfigFile config)
        {
            SetUpResolution();

            Config = config;
            SnapTurning = config.Bind(comfortCategory, "SnapTurning", false,
                "Snap turning");
            Teleport = config.Bind(comfortCategory, "Teleport", false,
                "Fixed camera while moving (\"teleport\" locomotion)");
            FixedCameraDuringAnimations = config.Bind(comfortCategory, "FixedCameraDuringAnimations", false,
                "Fixed camera during animations (experimental)");
            ShowLegs = config.Bind(playerBodyCategory, "ShowLegs", true,
                "Show ghost legs at all times");
            ControllerBasedMovementDirection = config.Bind(controlsCategory, "ControllerBasedMovementDirection", false,
                "Controller-based movement direction");
            LeftHandedMode = config.Bind(controlsCategory, "LeftHandedMode", false,
                "Left handed mode (might break some animations)");
            SwapSticks = config.Bind(controlsCategory, "SwapSticks", false,
                "Swap movement / rotation sticks");
        }

        private static void SetUpResolution()
        {
            Screen.SetResolution(1920, 1080, false);
        }
    }
}