using TwoForksVr.Settings;
using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public abstract class ActionInput<TAction> : IActionInput where TAction : ISteamVR_Action_In
    {
        protected readonly TAction SpecificAction;
        protected InputHandedness HandednessValue;
        protected string PromptSuffixValue;

        protected ActionInput(TAction action)
        {
            SpecificAction = action;
        }

        private SteamVR_Input_Sources HandSource
        {
            get
            {
                var isLeftHanded = VrSettings.LeftHandedMode.Value;
                switch (Handedness)
                {
                    case InputHandedness.Dominant:
                        return isLeftHanded ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand;
                    case InputHandedness.NonDominant:
                        return isLeftHanded ? SteamVR_Input_Sources.RightHand : SteamVR_Input_Sources.LeftHand;
                    case InputHandedness.Any:
                    default:
                        return SteamVR_Input_Sources.Any;
                }
            }
        }

        public InputHandedness Handedness => HandednessValue;
        public ISteamVR_Action_In Action => SpecificAction;
        public float AxisValue => GetAxisValue(HandSource);
        public bool ButtonValue => GetButtonValue(HandSource);
        public bool ButtonUp => GetButtonUp(HandSource);
        public bool ButtonDown => GetButtonDown(HandSource);
        public string PromptSuffix => PromptSuffixValue;

        public SteamVR_Input_Sources ActiveSource =>
            Action != null && Action.active ? Action.activeDevice : SteamVR_Input_Sources.Any;

        public float GetAxisValue(SteamVR_Input_Sources source)
        {
            return Action.active ? GetValue(source) : 0;
        }

        public bool GetButtonValue(SteamVR_Input_Sources source)
        {
            return Action.active && GetValue(source) != 0;
        }

        public bool GetButtonUp(SteamVR_Input_Sources source)
        {
            return Action.active && GetValueUp(source);
        }

        public bool GetButtonDown(SteamVR_Input_Sources source)
        {
            return Action.active && GetValueDown(source);
        }

        protected abstract float GetValue(SteamVR_Input_Sources source);
        protected abstract bool GetValueUp(SteamVR_Input_Sources source);
        protected abstract bool GetValueDown(SteamVR_Input_Sources source);
    }
}