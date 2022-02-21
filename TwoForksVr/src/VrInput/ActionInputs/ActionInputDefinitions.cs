using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public static class ActionInputDefinitions
    {
        public static readonly SteamVR_Input_ActionSet_default ActionSet = SteamVR_Actions._default;

        public static readonly BooleanActionInput Cancel =
            new BooleanActionInput(ActionSet.Cancel);

        public static readonly BooleanActionInput Radio =
            new BooleanActionInput(ActionSet.Radio, InputHandedness.NonDominant);

        public static readonly BooleanActionInput Interact =
            new BooleanActionInput(ActionSet.Interact, InputHandedness.Dominant);

        public static readonly BooleanActionInput Jog =
            new BooleanActionInput(ActionSet.Jog);

        public static readonly BooleanActionInput UIUp =
            new BooleanActionInput(ActionSet.UIUp);

        public static readonly BooleanActionInput UIDown =
            new BooleanActionInput(ActionSet.UIDown);

        public static readonly BooleanActionInput NextPage =
            new BooleanActionInput(ActionSet.NextPage);

        public static readonly BooleanActionInput PreviousPage =
            new BooleanActionInput(ActionSet.PreviousPage);

        public static readonly Vector2ActionInput MoveX =
            new Vector2ActionInput(ActionSet.Move);

        public static readonly Vector2ActionInput MoveY =
            new Vector2ActionInput(ActionSet.Move, true);

        public static readonly Vector2ActionInput RotateX =
            new Vector2ActionInput(ActionSet.Rotate);

        public static readonly BooleanActionInput Recenter =
            new BooleanActionInput(ActionSet.Recenter);

        public static readonly BooleanActionInput ToolPicker =
            new BooleanActionInput(ActionSet.ToolPicker);

        public static readonly BooleanActionInput Teleport =
            new BooleanActionInput(ActionSet.Teleport);

        public static readonly BooleanActionInput SnapTurnLeft =
            new BooleanActionInput(ActionSet.SnapTurnLeft);

        public static readonly BooleanActionInput SnapTurnRight =
            new BooleanActionInput(ActionSet.SnapTurnRight);

        public static readonly BooleanActionInput StoreItem =
            new BooleanActionInput(ActionSet.StoreItem);

        public static readonly BooleanActionInput LocomotionAction =
            new BooleanActionInput(ActionSet.LocomotionAction);
    }
}