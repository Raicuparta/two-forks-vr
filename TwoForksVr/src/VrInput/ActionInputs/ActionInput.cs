using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public abstract class ActionInput<TAction> : IActionInput where TAction : ISteamVR_Action_In
    {
        protected readonly TAction SpecificAction;
        protected bool IsEitherHandValue;
        protected string PromptSuffixValue;

        protected ActionInput(TAction action)
        {
            SpecificAction = action;
        }

        public ISteamVR_Action_In Action => SpecificAction;
        public float AxisValue => GetAxisValue();
        public bool ButtonValue => GetButtonValue();
        public bool ButtonUp => GetButtonUp();
        public bool ButtonDown => GetButtonDown();
        public bool IsEitherHand => IsEitherHandValue;
        public string PromptSuffix => PromptSuffixValue;

        public SteamVR_Input_Sources ActiveSource =>
            Action != null && Action.active ? Action.activeDevice : SteamVR_Input_Sources.Any;

        public float GetAxisValue(SteamVR_Input_Sources source = SteamVR_Input_Sources.Any)
        {
            return Action.active ? GetValue(source) : 0;
        }

        public bool GetButtonValue(SteamVR_Input_Sources source = SteamVR_Input_Sources.Any)
        {
            return Action.active && GetValue(source) != 0;
        }

        public bool GetButtonUp(SteamVR_Input_Sources source = SteamVR_Input_Sources.Any)
        {
            return Action.active && GetValueUp(source);
        }

        public bool GetButtonDown(SteamVR_Input_Sources source = SteamVR_Input_Sources.Any)
        {
            return Action.active && GetValueDown(source);
        }

        protected abstract float GetValue(SteamVR_Input_Sources source);
        protected abstract bool GetValueUp(SteamVR_Input_Sources source);
        protected abstract bool GetValueDown(SteamVR_Input_Sources source);
    }
}