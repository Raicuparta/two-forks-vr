using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public class SingleActionInput : ActionInput<SteamVR_Action_Single>
    {
        public SingleActionInput(SteamVR_Action_Single action) : base(action)
        {
        }

        public override float Value => SpecificAction.axis;
        public override bool ValueUp => false;
        public override bool ValueDown => false;
    }
}