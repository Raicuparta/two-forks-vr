using TwoForksVr.Settings;
using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs;

public abstract class ActionInput<TAction> : IActionInput where TAction : ISteamVR_Action_In
{
    protected readonly TAction SpecificAction;

    protected ActionInput(TAction action)
    {
        SpecificAction = action;
    }

    private SteamVR_Input_Sources HandSource
    {
        get
        {
            var isLeftHanded = VrSettings.LeftHandedMode.Value;
            var isSwappedSticks = VrSettings.SwapSticks.Value;
            if (SpecificAction.actionSet == SteamVR_Actions.DominantHand)
                return isLeftHanded ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand;
            if (SpecificAction.actionSet == SteamVR_Actions.NonDominantHand)
                return isLeftHanded ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand;
            if (SpecificAction.actionSet == SteamVR_Actions.RotationHand)
                return isSwappedSticks ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand;
            if (SpecificAction.actionSet == SteamVR_Actions.MovementHand)
                return isSwappedSticks ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand;
            return SteamVR_Input_Sources.Any;
        }
    }

    public ISteamVR_Action_In Action => SpecificAction;
    public float AxisValue => GetAxisValue(HandSource);
    public bool ButtonValue => GetButtonValue(HandSource);
    public bool ButtonUp => GetButtonUp(HandSource);
    public bool ButtonDown => GetButtonDown(HandSource);

    public SteamVR_Input_Sources ActiveSource
    {
        get
        {
            if (HandSource != SteamVR_Input_Sources.Any) return HandSource;

            return Action != null && Action.active ? Action.activeDevice : SteamVR_Input_Sources.Any;
        }
    }

    private float GetAxisValue(SteamVR_Input_Sources source)
    {
        return Action.active ? GetValue(source) : 0;
    }

    private bool GetButtonValue(SteamVR_Input_Sources source)
    {
        return Action.active && GetValue(source) != 0;
    }

    private bool GetButtonUp(SteamVR_Input_Sources source)
    {
        return Action.active && GetValueUp(source);
    }

    private bool GetButtonDown(SteamVR_Input_Sources source)
    {
        return Action.active && GetValueDown(source);
    }

    protected abstract float GetValue(SteamVR_Input_Sources source);
    protected abstract bool GetValueUp(SteamVR_Input_Sources source);
    protected abstract bool GetValueDown(SteamVR_Input_Sources source);
}