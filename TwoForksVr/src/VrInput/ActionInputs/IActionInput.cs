using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public interface IActionInput
    {
        ISteamVR_Action_In Action { get; }
        bool Optional { get; }
        bool Active { get; }
        float Value { get; }
        SteamVR_Input_Sources ActiveSource { get; }
    }
}