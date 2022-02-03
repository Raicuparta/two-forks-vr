using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public class BooleanActionInput : ActionInput<SteamVR_Action_Boolean>
    {
        private readonly bool isInverted;

        public BooleanActionInput(SteamVR_Action_Boolean action, bool inverted = false, bool clickable = true) :
            base(action)
        {
            Clickable = clickable;
            isInverted = inverted;
        }

        public override float Value => SpecificAction.state ? 1 : 0; // TODO inverted?
        public override bool ValueUp => SpecificAction.stateUp;
        public override bool ValueDown => SpecificAction.stateDown;
        public bool Clickable { get; }
    }
}