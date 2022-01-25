using BepInEx.Configuration;

namespace TwoForksVr.Settings
{
    public static class VrSettings
    {
        public static ConfigFile Config;
        public static ConfigEntry<bool> SnapTurning;
        public static ConfigEntry<bool> ShowFeet;
        public static ConfigEntry<bool> ShowBody;
        public static ConfigEntry<bool> Teleport;
        public static ConfigEntry<bool> FixedCameraDuringAnimations;

        public static void SetUp(ConfigFile config)
        {
            Config = config;
            SnapTurning = config.Bind("Config", "SnapTurning", false,
                "Snap turning");
            Teleport = config.Bind("Config", "Teleport", false,
                "Fixed camera whie moving (\"teleport\" locomotion)");
            FixedCameraDuringAnimations = config.Bind("Config", "FixedCameraDuringAnimations", false,
                "Fixed camera during player animations");
            ShowBody = config.Bind("Config", "ShowBody", false,
                "Show player body");
            ShowFeet = config.Bind("Config", "ShowFeet", true,
                "Show player feet");
        }
    }
}