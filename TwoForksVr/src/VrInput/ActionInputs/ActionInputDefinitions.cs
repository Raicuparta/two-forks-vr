using static Valve.VR.SteamVR_Actions;

namespace TwoForksVr.VrInput.ActionInputs
{
    public static class ActionInputDefinitions
    {
        public static readonly BooleanActionInput Cancel =
            new BooleanActionInput(NonDominantHand.Cancel);

        public static readonly BooleanActionInput Radio =
            new BooleanActionInput(NonDominantHand.Radio);

        public static readonly BooleanActionInput Interact =
            new BooleanActionInput(DominantHand.Interact);

        public static readonly BooleanActionInput Jog =
            new BooleanActionInput(MovementHand.Jog);

        public static readonly BooleanActionInput UiUp =
            new BooleanActionInput(RotationHand.UiUp);

        public static readonly BooleanActionInput UiDown =
            new BooleanActionInput(RotationHand.UiDown);

        public static readonly BooleanActionInput NextPage =
            new BooleanActionInput(RotationHand.NextPage);

        public static readonly BooleanActionInput PreviousPage =
            new BooleanActionInput(RotationHand.PreviousPage);

        public static readonly Vector2ActionInput MoveX =
            new Vector2ActionInput(MovementHand.Move);

        public static readonly Vector2ActionInput MoveY =
            new Vector2ActionInput(MovementHand.Move, true);

        public static readonly Vector2ActionInput RotateX =
            new Vector2ActionInput(RotationHand.Rotate);

        public static readonly BooleanActionInput ToolPicker =
            new BooleanActionInput(DominantHand.ToolPicker);

        public static readonly BooleanActionInput Teleport =
            new BooleanActionInput(MovementHand.Teleport);

        public static readonly BooleanActionInput SnapTurnLeft =
            new BooleanActionInput(RotationHand.SnapTurnLeft);

        public static readonly BooleanActionInput SnapTurnRight =
            new BooleanActionInput(RotationHand.SnapTurnRight);

        public static readonly BooleanActionInput StoreItem =
            new BooleanActionInput(DominantHand.StoreItem);

        public static readonly BooleanActionInput LocomotionAction =
            new BooleanActionInput(RotationHand.LocomotionAction);
    }
}