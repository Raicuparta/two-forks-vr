using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public class Vector2ActionInput : ActionInput<SteamVR_Action_Vector2>
    {
        private readonly bool yOnly;

        public Vector2ActionInput(SteamVR_Action_Vector2 action, bool yOnly = false,
            string textureModifier = null) : base(action)
        {
            this.yOnly = yOnly;
            TextureModifier = textureModifier;
        }

        public override float Value => yOnly ? SpecificAction.axis.y : SpecificAction.axis.x;
        public string TextureModifier { get; }
        public override bool ValueUp => false;
        public override bool ValueDown => false;
    }
}