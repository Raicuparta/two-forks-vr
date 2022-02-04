using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public class EmptyActionInput : ActionInput<ISteamVR_Action_In>
    {
        public EmptyActionInput(string texturePath = null) : base(null)
        {
            TexturePath = texturePath;
        }

        public override float Value => 0;
        public string TexturePath { get; }
        public override bool ValueUp => false;
        public override bool ValueDown => false;
    }
}