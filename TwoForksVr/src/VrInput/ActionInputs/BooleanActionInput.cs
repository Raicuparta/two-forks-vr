using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public class BooleanActionInput : ActionInput<SteamVR_Action_Boolean>
    {
        public BooleanActionInput(SteamVR_Action_Boolean action, string promptSuffix = "", bool isEitherHand = false) :
            base(action)
        {
            PromptSuffixValue = promptSuffix;
            IsEitherHandValue = isEitherHand;
        }

        protected override float GetValue()
        {
            return SpecificAction.state ? 1 : 0;
        }

        protected override bool GetValueUp()
        {
            return SpecificAction.stateUp;
        }

        protected override bool GetValueDown()
        {
            return SpecificAction.stateDown;
        }
    }
}