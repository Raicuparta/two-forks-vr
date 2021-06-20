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
        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            // TODO relative path
            OpenVR.Input.SetActionManifestPath(@"C:\Users\rai\Repos\two-forks-vr\HelperUnityProject\Build\Helper_Data\StreamingAssets\SteamVR\actions.json");
        }

        [HarmonyPatch(typeof(vgInputManager), "RegisterForInputCallback")]
        public class PatchTossAnimation
        {

            public static bool Prefix(string commandName, vgInputManager.InputDelegate callback)
            {
                var command = (InputCommand) Enum.Parse(typeof(InputCommand), commandName);
                var actionSet = SteamVR_Actions._default;

                switch (command) {
                    case InputCommand.Use:
                    {
                        actionSet.Interact.onChange += (SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) =>
                        {
                            callback(newState ? 1f : 0f);
                        };
                        return false;
                    }
                    case InputCommand.MoveForward:
                    {
                        actionSet.Move.onChange += (SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta) =>
                        {
                            callback(axis.y);
                        };
                        return false;
                    }
                    case InputCommand.MoveStrafe:
                    {
                        actionSet.Move.onChange += (SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta) =>
                        {
                            callback(axis.x);
                        };
                        return false;
                    }
                    case InputCommand.LookHorizontal_Stick:
                    {
                        actionSet.Rotate.onChange += (SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta) =>
                        {
                            callback(axis.x);
                        };
                        return false;
                    }
                    default:
                    {
                        return true;
                    }
                }
            }
        }
    }
}
