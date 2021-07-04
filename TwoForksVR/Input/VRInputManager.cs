using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Valve.VR;

namespace Raicuparta.TwoForksVR
{
    public class VRInputManager : MonoBehaviour
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
                { InputThing.Climb, actionSet.Interact },
                { InputThing.ChooseUp, actionSet.UIUp },
                { InputThing.ChooseDown, actionSet.UIDown },
                { InputThing.Jog, actionSet.Jog },
                { InputThing.Pause, actionSet.Cancel },
                { InputThing.Interact, actionSet.Interact },
                { InputThing.NextPage, actionSet.NextPage },
                { InputThing.PreviousPage, actionSet.PreviousPage },
            };
            vector2XActionMap = new Dictionary<string, SteamVR_Action_Vector2>()
            {
                { InputThing.MoveStrafe, actionSet.Move },
                { InputThing.LookHorizontal, actionSet.Rotate },
            };
            vector2YActionMap = new Dictionary<string, SteamVR_Action_Vector2>()
            {
                { InputThing.MoveForward, actionSet.Move },
                { InputThing.LookVertical, actionSet.Rotate },
            };
            return;
        }

        [HarmonyPatch(typeof(vgAxisData), "Update")]
        public class PatchKeyDataUpdate
        {

            public static void Postfix(List<string> ___names, ref float ___axisValue, ref float ___axisValueLastFrame)
            {
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
        public static class PatchButtonDataUpdate
        {

            public static void Postfix(
                List<string> ___names,
                ref bool ___keyUp,
                ref bool ___keyDown

            )
            {
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

            [HarmonyPatch(typeof(SteamVR_Input), "GetActionsFileFolder")]
            public static class PatchActionsFile
            {
                public static bool Prefix(ref string __result)
                {
                    __result = $"{Directory.GetCurrentDirectory()}/Mods/TwoForksVR/Bindings";
                    return false;
                }
            }
        }

        //[HarmonyPatch(typeof(vgKeyBind), "UpdatePressCommands")]
        //public class PatchPressCommands
        //{

        //    public static void Prefix(vgKeyData keyToCheck)
        //    {
        //        MelonLogger.Msg("## Press" + String.Join(", ", keyToCheck.names.ToArray()));
        //    }
        //}
        //[HarmonyPatch(typeof(vgKeyBind), "UpdateReleaseCommands")]
        //public class PatchReleaseCommands
        //{

        //    public static void Prefix(vgKeyData keyToCheck)
        //    {
        //        MelonLogger.Msg("## Release" + String.Join(", ", keyToCheck.names.ToArray()));
        //    }
        //}
        //[HarmonyPatch(typeof(vgKeyBind), "UpdateOtherCommands")]
        //public class PatchOtherCommands
        //{

        //    public static void Prefix(vgKeyData keyToCheck)
        //    {
        //        MelonLogger.Msg("## Other" + String.Join(", ", keyToCheck.names.ToArray()));
        //    }
        //}
    }
}