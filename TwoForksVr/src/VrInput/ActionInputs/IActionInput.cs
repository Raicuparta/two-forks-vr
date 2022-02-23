using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public interface IActionInput
    {
        ISteamVR_Action_In Action { get; }
        float AxisValue { get; }
        bool ButtonValue { get; }
        bool ButtonUp { get; }
        bool ButtonDown { get; }
        SteamVR_Input_Sources ActiveSource { get; }
    }
}