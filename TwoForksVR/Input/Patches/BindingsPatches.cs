using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Valve.VR;

namespace TwoForksVR.Input.Patches
{
    [HarmonyPatch]
    public static class InputPatches
    {
        private static SteamVR_Input_ActionSet_default actionSet;
        private static Dictionary<string, SteamVR_Action_Boolean> booleanActionMap;
        private static Dictionary<string, SteamVR_Action_Vector2> vector2XActionMap;
        private static Dictionary<string, SteamVR_Action_Vector2> vector2YActionMap;

        private static void Initialize()
        {
            actionSet = SteamVR_Actions._default;
            booleanActionMap = new Dictionary<string, SteamVR_Action_Boolean>()
            {
                {InputName.Climb, actionSet.Interact},
                {InputName.ChooseUp, actionSet.UIUp},
                {InputName.ChooseDown, actionSet.UIDown},
                {InputName.Jog, actionSet.Jog},
                {InputName.Pause, actionSet.Cancel},
                {InputName.Interact, actionSet.Interact},
                {InputName.NextPage, actionSet.NextPage},
                {InputName.PreviousPage, actionSet.PreviousPage}
            };
            vector2XActionMap = new Dictionary<string, SteamVR_Action_Vector2>()
            {
                {InputName.MoveStrafe, actionSet.Move},
                {InputName.LookHorizontal, actionSet.Rotate}
            };
            vector2YActionMap = new Dictionary<string, SteamVR_Action_Vector2>()
            {
                {InputName.MoveForward, actionSet.Move},
                {InputName.LookVertical, actionSet.Rotate}
            };

            // Pick dialog option with interact button.
            // TODO: move this somewhere else.
            actionSet.Interact.onStateDown += (fromAction, fromSource) =>
            {
                if (!vgDialogTreeManager.Instance) return;
                vgDialogTreeManager.Instance.OnConfirmDialogChoice();
                vgDialogTreeManager.Instance.ClearNonRadioDialogChoices();
            };
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgAxisData), nameof(vgAxisData.Update))]
        private static void ReadAxisValuesFromSteamVR(vgAxisData __instance)
        {
            if (!SteamVR_Input.initialized) return;

            if (actionSet == null)
            {
                Initialize();
            }

            foreach (var name in __instance.names)
            {
                if (vector2XActionMap.ContainsKey(name))
                {
                    __instance.axisValue = vector2XActionMap[name].axis.x;
                    __instance.axisValueLastFrame = vector2XActionMap[name].lastAxis.x;
                }
                else if (vector2YActionMap.ContainsKey(name))
                {
                    __instance.axisValue = vector2YActionMap[name].axis.y;
                    __instance.axisValueLastFrame = vector2YActionMap[name].lastAxis.y;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgButtonData), nameof(vgButtonData.Update))]
        private static void ReadButtonValuesFromSteamVR(vgButtonData __instance){
            if (!SteamVR_Input.initialized) return;

            if (actionSet == null)
            {
                Initialize();
            }

            foreach (var name in __instance.names.Where(name => booleanActionMap.ContainsKey(name)))
            {
                __instance.keyUp = booleanActionMap[name].stateUp;
                __instance.keyDown = booleanActionMap[name].stateDown;
            }
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamVR_Input), nameof(SteamVR_Input.GetActionsFileFolder))]
        private static bool GetActionsFileFromMod(ref string __result)
        {
            // TODO: could probably just use the streamingassets folder and avoid doing this?
            __result = $"{Directory.GetCurrentDirectory()}/BepInEx/plugins/TwoForksVR/Bindings";
            return false;
        }
    }
}