using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public class Vector2ActionInput : ActionInput<SteamVR_Action_Vector2>
    {
        private readonly bool yOnly;

        public Vector2ActionInput(SteamVR_Action_Vector2 action,
            bool yOnly = false,
            string textureModifier = null) : base(action)
        {
            this.yOnly = yOnly;
            TextureModifier = textureModifier;
        }

        public string TextureModifier { get; }

        protected override float GetValue(SteamVR_Input_Sources source)
        {
            var axis = SpecificAction.GetAxis(source);
            return yOnly ? axis.y : axis.x;
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