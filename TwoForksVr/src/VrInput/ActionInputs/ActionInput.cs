using System;
using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public abstract class ActionInput<TAction> : IActionInput where TAction : ISteamVR_Action_In
    {
        protected readonly TAction SpecificAction;

        protected ActionInput(TAction action, bool optional = false)
        {
            SpecificAction = action;
            Optional = optional;
        }

        public Action<float> OnChange { get; set; }

        public ISteamVR_Action_In Action => SpecificAction;
        public bool Optional { get; }
        public abstract bool Active { get; }
        public abstract float Value { get; }
        public abstract bool ValueUp { get; }
        public abstract bool ValueDown { get; }
        public abstract SteamVR_Input_Sources ActiveSource { get; }
    }
}