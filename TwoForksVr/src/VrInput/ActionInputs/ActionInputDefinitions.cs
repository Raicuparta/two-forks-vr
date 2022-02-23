using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public static class ActionInputDefinitions
    {
        public static readonly SteamVR_Input_ActionSet_perhand PerHandActionSet = SteamVR_Actions.perhand;
        public static readonly SteamVR_Input_ActionSet_mirrored MirroredActionSet = SteamVR_Actions.mirrored;

        public static readonly BooleanActionInput Cancel =
            new BooleanActionInput(PerHandActionSet.Cancel);

        public static readonly BooleanActionInput Radio =
            new BooleanActionInput(MirroredActionSet.Radio, InputHandedness.NonDominant);

        public static readonly BooleanActionInput Interact =
            new BooleanActionInput(MirroredActionSet.Interact, InputHandedness.Dominant);

        public static readonly BooleanActionInput Jog =
            new BooleanActionInput(MirroredActionSet.Jog, InputHandedness.MovementStick);

        public static readonly BooleanActionInput UIUp =
            new BooleanActionInput(PerHandActionSet.UIUp);

        public static readonly BooleanActionInput UIDown =
            new BooleanActionInput(PerHandActionSet.UIDown);

        public static readonly BooleanActionInput NextPage =
            new BooleanActionInput(PerHandActionSet.NextPage);

        public static readonly BooleanActionInput PreviousPage =
            new BooleanActionInput(PerHandActionSet.PreviousPage);

        public static readonly Vector2ActionInput MoveX =
            new Vector2ActionInput(MirroredActionSet.Move, InputHandedness.MovementStick);

        public static readonly Vector2ActionInput MoveY =
            new Vector2ActionInput(MirroredActionSet.Move, InputHandedness.MovementStick, true);

        public static readonly Vector2ActionInput RotateX =
            new Vector2ActionInput(MirroredActionSet.Rotate, InputHandedness.RotationStick);

        public static readonly BooleanActionInput Recenter =
            new BooleanActionInput(PerHandActionSet.Recenter);

        public static readonly BooleanActionInput ToolPicker =
            new BooleanActionInput(MirroredActionSet.ToolPicker, InputHandedness.RotationStick);

        public static readonly BooleanActionInput Teleport =
            new BooleanActionInput(PerHandActionSet.Teleport);

        public static readonly BooleanActionInput SnapTurnLeft =
            new BooleanActionInput(PerHandActionSet.SnapTurnLeft);

        public static readonly BooleanActionInput SnapTurnRight =
            new BooleanActionInput(PerHandActionSet.SnapTurnRight);

        public static readonly BooleanActionInput StoreItem =
            new BooleanActionInput(PerHandActionSet.StoreItem);

        public static readonly BooleanActionInput LocomotionAction =
            new BooleanActionInput(PerHandActionSet.LocomotionAction);
    }
}