using Harmony;
using MelonLoader;
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

            private static SteamVR_Action_Boolean.ChangeHandler OnChangeBoolean(vgInputManager.InputDelegate callback)
            {
                return (SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) =>
                {
                    if (newState)
                    {
                        callback();
                    }
                };
            }

            private static SteamVR_Action_Vector2.ChangeHandler OnChangeVector2Horizontal(vgInputManager.InputDelegate callback)
            {
                return (SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta) =>
                {
                    callback(axis.x);
                };
            }

            private static SteamVR_Action_Vector2.ChangeHandler OnChangeVector2Vertical(vgInputManager.InputDelegate callback)
            {
                return (SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta) =>
                {
                    callback(axis.y);
                };
            }

            public static bool Prefix(string commandName, vgInputManager.InputDelegate callback)
            {
                var command = (InputCommand) Enum.Parse(typeof(InputCommand), commandName);
                var actionSet = SteamVR_Actions._default;

                switch (command) {
                    case InputCommand.UIClick:
                    case InputCommand.UISubmit:
                    case InputCommand.Use:
                    {
                        actionSet.Interact.onChange += OnChangeBoolean(callback);
                        return false;
                    }
                    case InputCommand.UIVertical:
                    case InputCommand.MoveForward:
                    {
                        actionSet.Move.onChange += OnChangeVector2Vertical(callback);
                        return false;
                    }
                    case InputCommand.UIHorizontal:
                    case InputCommand.MoveStrafe:
                    {
                        actionSet.Move.onChange += OnChangeVector2Horizontal(callback);
                        return false;
                    }
                    case InputCommand.LookHorizontal_Stick:
                    {
                        actionSet.Rotate.onChange += OnChangeVector2Horizontal(callback);
                        return false;
                    }
                    case InputCommand.DialogSelectionScroll:
                    {

                        actionSet.Rotate.onChange += OnChangeVector2Vertical(callback);
                        return false;
                    }
                    case InputCommand.NextMenu:
                    {
                        actionSet.NextPage.onChange += OnChangeBoolean(callback);
                        return false;
                    }
                    case InputCommand.PreviousMenu:
                    {
                        actionSet.PreviousPage.onChange += OnChangeBoolean(callback);
                        return false;
                    }
                    case InputCommand.UICancel:
                    {
                        actionSet.Cancel.onChange += OnChangeBoolean(callback);
                        return false;
                    }
                    case InputCommand.ToggleJog:
                    {
                        actionSet.Jog.onChange += OnChangeBoolean(callback);
                        return false;
                    }
                    case InputCommand.Flashlight:
                    {
                        actionSet.ToolPicker.onChange += OnChangeBoolean(callback);
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
