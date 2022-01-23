using BepInEx.Configuration;

namespace TwoForksVr.Settings
{
    public static class VrSettings
    {
        public static ConfigFile Config;
        // public static ConfigEntry<bool> HandOrientedMovement;
        public static ConfigEntry<bool> SnapTurning;
        public static ConfigEntry<bool> ShowFeet;
        public static ConfigEntry<bool> ShowBody;

        public static void SetUp(ConfigFile config)
        {
            Config = config;
            // HandOrientedMovement = config.Bind("Config", "HandOrientedMovement", false,
            //     "Hand oriented movement");
            SnapTurning = config.Bind("Config", "SnapTurning", false,
                "Snap turning");
            ShowBody = config.Bind("Config", "ShowBody", false,
                "Show player body");
            ShowFeet = config.Bind("Config", "ShowFeet", true,
                "Show player feet");
        }
    }
}