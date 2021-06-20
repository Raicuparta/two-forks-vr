using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Valve.VR;

namespace Raicuparta.TwoForksVR
{
    public class VRInputManager : MonoBehaviour
    {
        [HarmonyPatch(typeof(vgInputManager), "RegisterForInputCallback")]
        public class PatchTossAnimation
        {
            public static bool Prefix(string commandName, vgInputManager.InputDelegate callback)
            {
                var command = (InputCommand) Enum.Parse(typeof(InputCommand), commandName);

                if (command == InputCommand.Use) {
                    SteamVR_Actions.default_InteractUI.onChange += (SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) =>
                    {
                        callback(newState ? 1f : 0f);
                    };
                    return false;
                }
                return true;
            }
        }
    }
}
