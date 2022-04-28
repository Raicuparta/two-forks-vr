using static Valve.VR.SteamVR_Actions;

namespace TwoForksVr.VrInput.ActionInputs;

public static class ActionInputDefinitions
{
    public static readonly BooleanActionInput Cancel = new(NonDominantHand.Cancel);

    public static readonly BooleanActionInput Radio = new(NonDominantHand.Radio);

    public static readonly BooleanActionInput Interact = new(DominantHand.Interact);

    public static readonly BooleanActionInput Jog = new(MovementHand.Jog);

    public static readonly BooleanActionInput UiUp = new(RotationHand.UiUp);

    public static readonly BooleanActionInput UiDown = new(RotationHand.UiDown);

    public static readonly BooleanActionInput NextPage = new(RotationHand.NextPage);

    public static readonly BooleanActionInput PreviousPage = new(RotationHand.PreviousPage);

    public static readonly Vector2ActionInput MoveX = new(MovementHand.Move);

    public static readonly Vector2ActionInput MoveY = new(MovementHand.Move, true);

    public static readonly Vector2ActionInput RotateX = new(RotationHand.Rotate);

    public static readonly BooleanActionInput ToolPicker = new(DominantHand.ToolPicker);

    public static readonly BooleanActionInput Teleport = new(MovementHand.Teleport);

    public static readonly BooleanActionInput SnapTurnLeft = new(RotationHand.SnapTurnLeft);

    public static readonly BooleanActionInput SnapTurnRight = new(RotationHand.SnapTurnRight);

    public static readonly BooleanActionInput StoreItem = new(DominantHand.StoreItem);

    public static readonly BooleanActionInput LocomotionAction = new(RotationHand.LocomotionAction);
}