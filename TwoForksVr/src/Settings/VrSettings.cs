using BepInEx.Configuration;

namespace TwoForksVr.Settings
{
    public static class VrSettings
    {
        private const string locomotionCategory = "Locomotion";
        private const string playerBodyCategory = "Player Body";
        private const string turningCategory = "Turning";

        public static ConfigFile Config { get; private set; }
        public static ConfigEntry<bool> SnapTurning { get; private set; }
        public static ConfigEntry<bool> ShowLegs { get; private set; }
        public static ConfigEntry<bool> UseOriginalHandsWhileNavigationDisabled { get; private set; }
        public static ConfigEntry<bool> Teleport { get; private set; }
        public static ConfigEntry<bool> FixedCameraDuringAnimations { get; private set; }

        public static void SetUp(ConfigFile config)
        {
            Config = config;
            SnapTurning = config.Bind(turningCategory, "SnapTurning", false,
                "Snap turning");
            Teleport = config.Bind(locomotionCategory, "Teleport", false,
                "Fixed camera while moving (\"teleport\" locomotion)");
            FixedCameraDuringAnimations = config.Bind(locomotionCategory, "FixedCameraDuringAnimations", false,
                "Fixed camera during animations (experimental)");
            ShowLegs = config.Bind(playerBodyCategory, "ShowLegs", true,
                "Show ghost legs at all times");
            UseOriginalHandsWhileNavigationDisabled = config.Bind(playerBodyCategory,
                "UseOriginalHandsWhileNavigationDisabled", true,
                "Enable ghost hands during some animations");
        }
    }
}