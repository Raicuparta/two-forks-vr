using BepInEx.Configuration;

namespace TwoForksVr.Settings
{
    public static class VrSettings
    {
        public static ConfigFile Config;
        // public static ConfigEntry<bool> HandOrientedMovement;
        // public static ConfigEntry<bool> SnapTurning;
        public static ConfigEntry<bool> HideFeet;

        public static void SetUp(ConfigFile config)
        {
            Config = config;
            // HandOrientedMovement = config.Bind("Config", "HandOrientedMovement", false,
            //     "Hand oriented movement");
            // SnapTurning = config.Bind("Config", "SnapTurning", false,
            //     "Snap turning");
            HideFeet = config.Bind("Config", "HideFeet", false,
                "Hide player feet");
        }
    }
}