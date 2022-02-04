using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public class BooleanActionInput : ActionInput<SteamVR_Action_Boolean>
    {
        public BooleanActionInput(SteamVR_Action_Boolean action) :
            base(action)
        {
        }

        public override float Value => SpecificAction.state ? 1 : 0;
        public override bool ValueUp => SpecificAction.stateUp;
        public override bool ValueDown => SpecificAction.stateDown;
    }
}