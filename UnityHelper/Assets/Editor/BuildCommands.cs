using System;
using UnityEditor;

/// <summary>
/// Set of instructions for automated builds
/// </summary>
public static class BuildCommands
{
    static void PerformBuild ()
    {
        var buildPlayerOptions = new BuildPlayerOptions
        {
            locationPathName = "Build/UnityHelper.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        EditorUserBuildSettings.development = false;

        var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        EditorApplication.Exit(String.IsNullOrEmpty(buildReport) ? 0 : 1);
    }
}