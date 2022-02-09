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
        bool IsEitherHand { get; }
        string PromptSuffix { get; }
        SteamVR_Input_Sources ActiveSource { get; }

        float GetAxisValue(SteamVR_Input_Sources source);
        bool GetButtonValue(SteamVR_Input_Sources source);
        bool GetButtonUp(SteamVR_Input_Sources source);
        bool GetButtonDown(SteamVR_Input_Sources source);
    }
}