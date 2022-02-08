using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public class EmptyActionInput : ActionInput<ISteamVR_Action_In>
    {
        public EmptyActionInput(string texturePath = null) : base(null)
        {
            TexturePath = texturePath;
        }

        public string TexturePath { get; }

        protected override float GetValue(SteamVR_Input_Sources source)
        {
            return 0;
        }

        protected override bool GetValueUp(SteamVR_Input_Sources source)
        {
            return false;
        }

        protected override bool GetValueDown(SteamVR_Input_Sources source)
        {
            return false;
        }
    }
}