using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Valve.Newtonsoft.Json;
using Valve.VR;

namespace TwoForksVr.Helpers
{
    public static class ApplicationManifestHelper
    {
        public static void UpdateManifest(string manifestPath, string appKey, string imagePath, string name,
            string description, int steamAppId = 0, bool steamBuild = false)
        {
            var launchType = steamBuild ? GetSteamLaunchString(steamAppId) : GetBinaryLaunchString();
            var appManifestContent = $@"{{
                                            ""source"": ""builtin"",
                                            ""applications"": [{{
                                                ""app_key"": {JsonConvert.ToString(appKey)},
                                                ""image_path"": {JsonConvert.ToString(imagePath)},
                                                {launchType}
                                                ""last_played_time"":""{CurrentUnixTimestamp()}"",
                                                ""strings"": {{
                                                    ""en_us"": {{
                                                        ""name"": {JsonConvert.ToString(name)}
                                                    }}
                                                }}
                                            }}]
                                        }}";

            File.WriteAllText(manifestPath, appManifestContent);

            var error = OpenVR.Applications.AddApplicationManifest(manifestPath, false);
            if (error != EVRApplicationError.None) Logs.LogError("Failed to set AppManifest " + error);

            var processId = Process.GetCurrentProcess().Id;
            var applicationIdentifyErr = OpenVR.Applications.IdentifyApplication((uint) processId, appKey);
            if (applicationIdentifyErr != EVRApplicationError.None)
                Logs.LogError("Error identifying application: " + applicationIdentifyErr);
        }

        private static string GetSteamLaunchString(int steamAppId)
        {
            return $@"""launch_type"": ""url"",
                      ""url"": ""steam://launch/{steamAppId}/VR"",";
        }

        private static string GetBinaryLaunchString()
        {
            var workingDir = Directory.GetCurrentDirectory();
            var executablePath = Assembly.GetExecutingAssembly().Location;
            return $@"""launch_type"": ""binary"",
                      ""binary_path_windows"": {JsonConvert.ToString(executablePath)},
                      ""working_directory"": {JsonConvert.ToString(workingDir)},";
        }

        private static double CurrentUnixTimestamp()
        {
            return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}