using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public abstract class ActionInput<TAction> : IActionInput where TAction : ISteamVR_Action_In
    {
        protected readonly TAction SpecificAction;

        protected ActionInput(TAction action)
        {
            SpecificAction = action;
        }

        public ISteamVR_Action_In Action => SpecificAction;
        public bool Active => Action.active;
        public abstract float Value { get; }
        public abstract bool ValueUp { get; }
        public abstract bool ValueDown { get; }
        public SteamVR_Input_Sources ActiveSource => Action?.activeDevice ?? SteamVR_Input_Sources.Any;
    }
}