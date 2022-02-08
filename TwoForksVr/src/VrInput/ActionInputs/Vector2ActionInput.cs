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

        public string TextureModifier { get; }

        protected override float GetValue()
        {
            return yOnly ? SpecificAction.axis.y : SpecificAction.axis.x;
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