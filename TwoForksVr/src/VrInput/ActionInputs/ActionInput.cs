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
        public float AxisValue => Action.active ? GetValue() : 0;
        public bool ButtonValue => Action.active && GetValue() != 0;
        public bool ButtonUp => Action.active && GetValueUp();
        public bool ButtonDown => Action.active && GetValueDown();
        public bool IsEitherHand => IsEitherHandValue;
        public string PromptSuffix => PromptSuffixValue;

        public SteamVR_Input_Sources ActiveSource =>
            Action != null && Action.active ? Action.activeDevice : SteamVR_Input_Sources.Any;

        protected abstract float GetValue();
        protected abstract bool GetValueUp();
        protected abstract bool GetValueDown();
    }
}