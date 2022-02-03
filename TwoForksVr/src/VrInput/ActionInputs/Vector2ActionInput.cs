using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public class Vector2ActionInput : ActionInput<SteamVR_Action_Vector2>
    {
        private readonly bool clamp;
        private readonly bool invert;
        private readonly bool isEitherHand;
        private readonly bool yOnly;
        private readonly bool yZero;

        public Vector2ActionInput(SteamVR_Action_Vector2 action, bool optional = false, bool yOnly = false,
            bool invert = false, bool clamp = false, bool yZero = false, bool eitherHand = false,
            string textureModifier = null) : base(action, optional)
        {
            this.yOnly = yOnly;
            this.invert = invert;
            this.clamp = clamp;
            this.yZero = yZero;
            isEitherHand = eitherHand;
            TextureModifier = textureModifier;
        }

        public override bool Active
        {
            get
            {
                var state = isEitherHand
                    ? SpecificAction.GetActive(SteamVR_Input_Sources.LeftHand) ||
                      SpecificAction.GetActive(SteamVR_Input_Sources.RightHand)
                    : SpecificAction.active;
                return state;
            }
        }

        public override float Value => yOnly ? SpecificAction.axis.y : SpecificAction.axis.x;

        public override SteamVR_Input_Sources ActiveSource
        {
            get
            {
                var state = isEitherHand
                    ? (SteamVR_Input_Sources) ((int) SpecificAction.GetActiveDevice(SteamVR_Input_Sources.LeftHand) +
                                               (int) SpecificAction.GetActiveDevice(SteamVR_Input_Sources.RightHand))
                    : SpecificAction.activeDevice;
                return state;
            }
        }

        public string TextureModifier { get; }
    }
}