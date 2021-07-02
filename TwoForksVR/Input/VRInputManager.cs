using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
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
        public class PatchButtonDataUpdate
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
        }
    }
}

/**

from vgKeyBind.UpdatePressCommands

select (notes): tab
start (pause): escape
A (climb): space, enter
X (jog): r
Y (read note text): q
B (keep held item): e, escape
dpad up (map): DPadVertical -> these two are the same?
dpad up (compass): DPadVertical -> these two are the same?
dpad right (camera): DPadHorizontal
right stick click (flashlight): f, enter
left stick click (jog): r
left stick y (move): MoveForward
left stick x (move): MoveStrafe
right stick y (look): LookVertical_Stick
right stick x (look): LookHorizontal_Stick
rt (choose dialogue / use): RightTrigger
lt (radio): LeftTrigger
rb (choose dialogue): up, e
lb (zoom / examine held item): mouse 1, q

**/