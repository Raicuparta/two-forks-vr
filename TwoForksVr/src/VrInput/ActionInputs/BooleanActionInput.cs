using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs;

public class BooleanActionInput : ActionInput<SteamVR_Action_Boolean>
{
    public BooleanActionInput(SteamVR_Action_Boolean action) :
        base(action)
    {
    }

    protected override float GetValue(SteamVR_Input_Sources source)
    {
        return SpecificAction.GetState(source) ? 1 : 0;
    }

    protected override bool GetValueUp(SteamVR_Input_Sources source)
    {
        return SpecificAction.GetStateUp(source);
    }

    protected override bool GetValueDown(SteamVR_Input_Sources source)
    {
        return SpecificAction.GetStateDown(source);
    }
}