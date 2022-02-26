using Valve.VR;

namespace TwoForksVr.VrInput.ActionInputs
{
    public static class ActionInputDefinitions
    {
        // TODO: would be a lot cleaner to just do a static import of the action sets,
        // but if I don't do these declarations here I get steamvr errors. I think I need to initialize it sooner.
        private static readonly SteamVR_Input_ActionSet_NonDominantHand nonDominant = SteamVR_Actions.NonDominantHand;
        private static readonly SteamVR_Input_ActionSet_DominantHand dominant = SteamVR_Actions.DominantHand;
        private static readonly SteamVR_Input_ActionSet_MovementHand movement = SteamVR_Actions.MovementHand;
        private static readonly SteamVR_Input_ActionSet_RotationHand rotation = SteamVR_Actions.RotationHand;
        private static readonly SteamVR_Input_ActionSet_perhand perHand = SteamVR_Actions.perhand;

        public static readonly BooleanActionInput Cancel =
            new BooleanActionInput(nonDominant.Cancel);

        public static readonly BooleanActionInput Radio =
            new BooleanActionInput(nonDominant.Radio);

        public static readonly BooleanActionInput Interact =
            new BooleanActionInput(dominant.Interact);

        public static readonly BooleanActionInput Jog =
            new BooleanActionInput(movement.Jog);

        public static readonly BooleanActionInput UIUp =
            new BooleanActionInput(perHand.UIUp);

        public static readonly BooleanActionInput UIDown =
            new BooleanActionInput(perHand.UIDown);

        public static readonly BooleanActionInput NextPage =
            new BooleanActionInput(perHand.NextPage);

        public static readonly BooleanActionInput PreviousPage =
            new BooleanActionInput(perHand.PreviousPage);

        public static readonly Vector2ActionInput MoveX =
            new Vector2ActionInput(movement.Move);

        public static readonly Vector2ActionInput MoveY =
            new Vector2ActionInput(movement.Move, true);

        public static readonly Vector2ActionInput RotateX =
            new Vector2ActionInput(rotation.Rotate);

        public static readonly BooleanActionInput Recenter =
            new BooleanActionInput(perHand.Recenter);

        public static readonly BooleanActionInput ToolPicker =
            new BooleanActionInput(dominant.ToolPicker);

        public static readonly BooleanActionInput Teleport =
            new BooleanActionInput(movement.Teleport);

        public static readonly BooleanActionInput SnapTurnLeft =
            new BooleanActionInput(rotation.SnapTurnLeft);

        public static readonly BooleanActionInput SnapTurnRight =
            new BooleanActionInput(rotation.SnapTurnRight);

        public static readonly BooleanActionInput StoreItem =
            new BooleanActionInput(dominant.StoreItem);

        public static readonly BooleanActionInput LocomotionAction =
            new BooleanActionInput(rotation.LocomotionAction);
    }
}