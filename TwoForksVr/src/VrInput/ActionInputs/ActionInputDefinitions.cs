namespace TwoForksVr.VrInput.ActionInputs
{
    public static class ActionInputDefinitions
    {
        public static readonly EmptyActionInput Empty =
            new EmptyActionInput();

        public static readonly BooleanActionInput Cancel =
            new BooleanActionInput(BindingsManager.ActionSet.Cancel);

        public static readonly BooleanActionInput Grip =
            new BooleanActionInput(BindingsManager.ActionSet.Grip);

        public static readonly BooleanActionInput Radio =
            new BooleanActionInput(BindingsManager.ActionSet.Radio);

        public static readonly BooleanActionInput Interact =
            new BooleanActionInput(BindingsManager.ActionSet.Interact, "", true);

        public static readonly BooleanActionInput Jog =
            new BooleanActionInput(BindingsManager.ActionSet.Jog);

        public static readonly BooleanActionInput UIUp =
            new BooleanActionInput(BindingsManager.ActionSet.UIUp, "^");

        public static readonly BooleanActionInput UIDown =
            new BooleanActionInput(BindingsManager.ActionSet.UIDown, "v");

        public static readonly BooleanActionInput NextPage =
            new BooleanActionInput(BindingsManager.ActionSet.NextPage, ">");

        public static readonly BooleanActionInput PreviousPage =
            new BooleanActionInput(BindingsManager.ActionSet.PreviousPage, "<");

        public static readonly Vector2ActionInput MoveX =
            new Vector2ActionInput(BindingsManager.ActionSet.Move);

        public static readonly Vector2ActionInput MoveY =
            new Vector2ActionInput(BindingsManager.ActionSet.Move, true);

        public static readonly Vector2ActionInput RotateX =
            new Vector2ActionInput(BindingsManager.ActionSet.Rotate);

        public static readonly Vector2ActionInput RotateY =
            new Vector2ActionInput(BindingsManager.ActionSet.Rotate, true);

        public static readonly BooleanActionInput Recenter =
            new BooleanActionInput(BindingsManager.ActionSet.Recenter);
    }
}