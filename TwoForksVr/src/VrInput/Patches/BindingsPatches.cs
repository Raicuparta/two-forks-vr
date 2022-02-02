using System.IO;
using HarmonyLib;
using TwoForksVr.Settings;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.VrInput.Patches
{
    [HarmonyPatch]
    public class BindingsPatches : TwoForksVrPatch
    {
        private const float outerDeadzone = 0.5f;
        private const float innerDeadzone = 0.1f;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamVR_Input), nameof(SteamVR_Input.GetActionsFileFolder))]
        private static bool GetActionsFileFromMod(ref string __result)
        {
            __result = $"{Directory.GetCurrentDirectory()}/BepInEx/plugins/TwoForksVrAssets/Bindings";
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.CheckForPCControls))]
        private static bool DisableJogDisableThreshold(vgPlayerController __instance)
        {
            __instance.minimumInputForJog = 0f;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.ForwardMovement))]
        private static bool FixForwardMovement(vgPlayerController __instance, float axisValue)
        {
            if (VrSettings.Teleport.Value && __instance.navController && __instance.navController.enabled) return false;

            __instance.forwardInput = ProcessAxisValue(axisValue);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.StrafeMovement))]
        private static bool FixStrafeMovement(vgPlayerController __instance, float axisValue)
        {
            if (VrSettings.Teleport.Value) return false;

            __instance.strafeInput = ProcessAxisValue(axisValue);
            return false;
        }

        private static float ProcessAxisValue(float value)
        {
            var valueSign = Mathf.Sign(value);
            var absoluteValue = Mathf.Abs(value);
            return valueSign * Mathf.InverseLerp(innerDeadzone, 1f - outerDeadzone, absoluteValue);
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgKeyBind), nameof(vgKeyBind.TriggerCommand))]
        private static bool IgnoreDefaultAxisInputs(string command)
        {
            return !StageInstance.IsVector2CommandExisting(command);
        }
    }
}