namespace TwoForksVr.VrInput.ActionInputs
{
    public enum InputHandedness
    {
        Any,

        // These are swapped with the LeftHandedMode setting.
        Dominant,
        NonDominant,

        // These are swapped with the SwapSticks setting.
        RotationStick,
        MovementStick
    }
}