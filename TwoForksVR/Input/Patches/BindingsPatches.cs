using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using Valve.VR;

namespace TwoForksVR.Input
{
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
                { InputName.Climb, actionSet.Interact },
                { InputName.ChooseUp, actionSet.UIUp },
                { InputName.ChooseDown, actionSet.UIDown },
                { InputName.Jog, actionSet.Jog },
                { InputName.Pause, actionSet.Cancel },
                { InputName.Interact, actionSet.Interact },
                { InputName.NextPage, actionSet.NextPage },
                { InputName.PreviousPage, actionSet.PreviousPage },
            };
            vector2XActionMap = new Dictionary<string, SteamVR_Action_Vector2>()
            {
                { InputName.MoveStrafe, actionSet.Move },
                { InputName.LookHorizontal, actionSet.Rotate },
            };
            vector2YActionMap = new Dictionary<string, SteamVR_Action_Vector2>()
            {
                { InputName.MoveForward, actionSet.Move },
                { InputName.LookVertical, actionSet.Rotate },
            };

            // Pick dialog option with interact button.
            // TODO: move this somewhere else.
            actionSet.Interact.onStateDown += (SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) =>
            {
                vgDialogTreeManager.Instance?.OnConfirmDialogChoice();
                vgDialogTreeManager.Instance?.ClearNonRadioDialogChoices();
            };

            return;
        }

        [HarmonyPatch(typeof(vgAxisData), "Update")]
        public class ReadAxisValuesFromSteamVR
        {
            public static void Postfix(List<string> ___names, ref float ___axisValue, ref float ___axisValueLastFrame)
            {
                if (!SteamVR_Input.initialized)
                {
                    return;
                }

                if (actionSet == null)
                {
                    Initialize();
                }

                foreach (var name in ___names)
                {
                    if (vector2XActionMap.ContainsKey(name))
                    {
                        ___axisValue = vector2XActionMap[name].axis.x;
                        ___axisValueLastFrame = vector2XActionMap[name].lastAxis.x;
                    }
                    else if (vector2YActionMap.ContainsKey(name))
                    {
                        ___axisValue = vector2YActionMap[name].axis.y;
                        ___axisValueLastFrame = vector2YActionMap[name].lastAxis.y;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(vgButtonData), "Update")]
        public static class ReadButtonValuesFromSteamVR
        {
            public static void Postfix(
                List<string> ___names,
                ref bool ___keyUp,
                ref bool ___keyDown

            )
            {
                if (!SteamVR_Input.initialized)
                {
                    return;
                }

                if (actionSet == null)
                {
                    Initialize();
                }

                foreach (var name in ___names)
                {
                    if (booleanActionMap.ContainsKey(name))
                    {
                        ___keyUp = booleanActionMap[name].stateUp;
                        ___keyDown = booleanActionMap[name].stateDown;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SteamVR_Input), "GetActionsFileFolder")]
        public static class GetActionsFileFromMod
        {
            public static bool Prefix(ref string __result)
            {
                __result = $"{Directory.GetCurrentDirectory()}/BepInEx/plugins/TwoForksVR/Bindings";
                return false;
            }
        }
    }
}