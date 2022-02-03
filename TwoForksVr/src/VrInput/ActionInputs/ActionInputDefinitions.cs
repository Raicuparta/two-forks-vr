using static Valve.VR.SteamVR_Actions;

namespace TwoForksVr.VrInput.ActionInputs
{
    public static class ActionInputDefinitions
    {
        public static readonly EmptyActionInput Empty =
            new EmptyActionInput();

        public static readonly BooleanActionInput Cancel =
            new BooleanActionInput(_default.Cancel);

        public static readonly BooleanActionInput Grip =
            new BooleanActionInput(_default.Grip);

        public static readonly BooleanActionInput Radio =
            new BooleanActionInput(_default.Radio);

        public static readonly BooleanActionInput Interact =
            new BooleanActionInput(_default.Interact);

        public static readonly BooleanActionInput Jog =
            new BooleanActionInput(_default.Jog);

        public static readonly BooleanActionInput UIUp =
            new BooleanActionInput(_default.UIUp);

        public static readonly BooleanActionInput UIDown =
            new BooleanActionInput(_default.UIDown);

        public static readonly BooleanActionInput NextPage =
            new BooleanActionInput(_default.NextPage);

        public static readonly BooleanActionInput PreviousPage =
            new BooleanActionInput(_default.PreviousPage);

        public static readonly Vector2ActionInput MoveX =
            new Vector2ActionInput(_default.Move);

        public static readonly Vector2ActionInput MoveY =
            new Vector2ActionInput(_default.Move, yOnly: true);

        public static readonly Vector2ActionInput RotateX =
            new Vector2ActionInput(_default.Rotate);

        public static readonly Vector2ActionInput RotateY =
            new Vector2ActionInput(_default.Rotate, yOnly: true);

        public static readonly BooleanActionInput Recenter =
            new BooleanActionInput(_default.Recenter);
    }
}