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
        private static Dictionary<vgInputManager.InputDelegate, SteamVR_Action_Boolean.ChangeHandler> booleanDelegateHandlerMap = new Dictionary<vgInputManager.InputDelegate, SteamVR_Action_Boolean.ChangeHandler>();
        private static Dictionary<vgInputManager.InputDelegate, SteamVR_Action_Vector2.ChangeHandler> vector2DelegateHandlerMap = new Dictionary<vgInputManager.InputDelegate, SteamVR_Action_Vector2.ChangeHandler>();

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            // TODO relative path
            OpenVR.Input.SetActionManifestPath(@"C:\Users\rai\Repos\two-forks-vr\TwoForksVR\Input\Bindings\actions.json");
        }

        [HarmonyPatch(typeof(vgInputManager), "RegisterForInputCallback")]
        public class PatchRegisterInput
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
                var command = (InputCommand)Enum.Parse(typeof(InputCommand), commandName);
                var actionSet = SteamVR_Actions._default;

                switch (command)
                {
                    case InputCommand.UIClick:
                    case InputCommand.UISubmit:
                    case InputCommand.Use:
                    {
                        var changeHandler = OnChangeBoolean(callback);
                        booleanDelegateHandlerMap.Add(callback, changeHandler);
                        actionSet.Interact.onChange += OnChangeBoolean(callback);
                        return false;
                    }
                    case InputCommand.MoveForward:
                    {
                        var changeHandler = OnChangeVector2Vertical(callback);
                        vector2DelegateHandlerMap.Add(callback, changeHandler);
                        actionSet.Move.onChange += changeHandler;
                        return false;
                    }
                    case InputCommand.UIHorizontal:
                    case InputCommand.MoveStrafe:
                    {
                        var changeHandler = OnChangeVector2Horizontal(callback);
                        vector2DelegateHandlerMap.Add(callback, changeHandler);
                        actionSet.Move.onChange += changeHandler;
                        return false;
                    }
                    case InputCommand.LookHorizontal_Stick:
                    {
                        var changeHandler = OnChangeVector2Horizontal(callback);
                        vector2DelegateHandlerMap.Add(callback, changeHandler);
                        actionSet.Rotate.onChange += changeHandler;
                        return false;
                    }
                    case InputCommand.NextMenu:
                    {
                        var changeHandler = OnChangeBoolean(callback);
                        booleanDelegateHandlerMap.Add(callback, changeHandler);
                        actionSet.NextPage.onChange += changeHandler;
                        return false;
                    }
                    case InputCommand.PreviousMenu:
                    {
                        var changeHandler = OnChangeBoolean(callback);
                        booleanDelegateHandlerMap.Add(callback, changeHandler);
                        actionSet.PreviousPage.onChange += changeHandler;
                        return false;
                    }
                    case InputCommand.UICancel:
                    case InputCommand.Pause:
                    case InputCommand.UnPause:
                    {
                        var changeHandler = OnChangeBoolean(callback);
                        booleanDelegateHandlerMap.Add(callback, changeHandler);
                        actionSet.Cancel.onChange += changeHandler;
                        return false;
                    }
                    case InputCommand.ToggleJog:
                    {
                        var changeHandler = OnChangeBoolean(callback);
                        booleanDelegateHandlerMap.Add(callback, changeHandler);
                        actionSet.Jog.onChange += changeHandler;
                        return false;
                    }
                    case InputCommand.UIUp:
                    case InputCommand.DialogSelectionUp:
                    {
                        var changeHandler = OnChangeBoolean(callback);
                        booleanDelegateHandlerMap.Add(callback, changeHandler);
                        actionSet.UIUp.onChange += changeHandler;
                        return false;
                    }
                    case InputCommand.UIDown:
                    case InputCommand.DialogSelectionDown:
                    {
                        var changeHandler = OnChangeBoolean(callback);
                        booleanDelegateHandlerMap.Add(callback, changeHandler);
                        actionSet.UIDown.onChange += changeHandler;
                        return false;
                    }
                    default:
                    {
                        return true;
                    }
                }
            }

        }

        [HarmonyPatch(typeof(vgInputManager), "UnregisterForInputCallback")]
        public class PatchUnregisterInput
        {
            public static bool Prefix(string commandName, vgInputManager.InputDelegate callback)
            {
                var command = (InputCommand)Enum.Parse(typeof(InputCommand), commandName);
                var actionSet = SteamVR_Actions._default;

                switch (command)
                {
                    case InputCommand.UIClick:
                    case InputCommand.UISubmit:
                    case InputCommand.Use:
                    {
                        actionSet.Interact.onChange -= booleanDelegateHandlerMap[callback];
                        booleanDelegateHandlerMap.Remove(callback);
                        return false;
                    }
                    case InputCommand.MoveForward:
                    {
                        actionSet.Move.onChange -= vector2DelegateHandlerMap[callback];
                        vector2DelegateHandlerMap.Remove(callback);
                        return false;
                    }
                    case InputCommand.UIHorizontal:
                    case InputCommand.MoveStrafe:
                    {
                        actionSet.Move.onChange -= vector2DelegateHandlerMap[callback];
                        vector2DelegateHandlerMap.Remove(callback);
                        return false;
                    }
                    case InputCommand.LookHorizontal_Stick:
                    {
                        actionSet.Rotate.onChange -= vector2DelegateHandlerMap[callback];
                        vector2DelegateHandlerMap.Remove(callback);
                        return false;
                    }
                    case InputCommand.NextMenu:
                    {
                        actionSet.NextPage.onChange -= booleanDelegateHandlerMap[callback];
                        booleanDelegateHandlerMap.Remove(callback);
                        return false;
                    }
                    case InputCommand.PreviousMenu:
                    {
                        actionSet.PreviousPage.onChange -= booleanDelegateHandlerMap[callback];
                        booleanDelegateHandlerMap.Remove(callback);
                        return false;
                    }
                    case InputCommand.UICancel:
                    case InputCommand.Pause:
                    case InputCommand.UnPause:
                    {
                        actionSet.Cancel.onChange -= booleanDelegateHandlerMap[callback];
                        booleanDelegateHandlerMap.Remove(callback);
                        return false;
                    }
                    case InputCommand.ToggleJog:
                    {
                        actionSet.Jog.onChange -= booleanDelegateHandlerMap[callback];
                        booleanDelegateHandlerMap.Remove(callback);
                        return false;
                    }
                    case InputCommand.UIUp:
                    case InputCommand.DialogSelectionUp:
                    {
                        actionSet.UIUp.onChange -= booleanDelegateHandlerMap[callback];
                        booleanDelegateHandlerMap.Remove(callback);
                        return false;
                    }
                    case InputCommand.UIDown:
                    case InputCommand.DialogSelectionDown:
                    {
                        actionSet.UIDown.onChange -= booleanDelegateHandlerMap[callback];
                        booleanDelegateHandlerMap.Remove(callback);
                        return false;
                    }
                    default:
                    {
                        return true;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(vgAxisData), MethodType.Constructor, new Type[] { typeof(string), typeof(float) })]
        public class PatchAddKey
        {
            public static void Prefix(string name)
            {
                MelonLogger.Msg("#### yoyoyo this is addkey SINGLE " + name);
            }
        }

        [HarmonyPatch(typeof(vgAxisData), MethodType.Constructor, new Type[] { typeof(List<string>), typeof(float) })]
        public class PatchAddKeyList
        {
            public static void Prefix(List<string> names)
            {
                MelonLogger.Msg("#### yoyoyo this is addkey LIST " + String.Join(", ", names.ToArray()));
            }
        }

        [HarmonyPatch(typeof(vgKeyBind), "UpdatePressCommands")]
        public class PatchTriggerCommand
        {
            public static void Prefix(vgKeyData keyToCheck)
            {
                MelonLogger.Msg("#### yoyoyo this is TriggerCommand " + keyToCheck.names[0]);
            }
        }
    }
}

/**

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