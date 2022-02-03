using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public interface IActionInput
    {
        ISteamVR_Action_In Action { get; }
        bool Active { get; }
        float Value { get; }
        bool ValueUp { get; }
        bool ValueDown { get; }
        SteamVR_Input_Sources ActiveSource { get; }
    }
}