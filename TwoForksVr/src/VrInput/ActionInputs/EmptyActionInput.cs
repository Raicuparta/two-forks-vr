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

        protected override float GetValue()
        {
            return 0;
        }

        protected override bool GetValueUp()
        {
            return false;
        }

        protected override bool GetValueDown()
        {
            return false;
        }
    }
}