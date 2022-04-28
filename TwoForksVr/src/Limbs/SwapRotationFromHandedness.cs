using TwoForksVr.Settings;
using UnityEngine;

namespace TwoForksVr.Limbs;

public class SwapRotationFromHandedness : HandednessChangeListener
{
    private Quaternion leftHandedRotation;
    private Quaternion rightHandedRotation;

    public static void Create(Transform transform, Quaternion rightHandedRotation, Quaternion leftHandedRotation)
    {
        var instance = transform.gameObject.AddComponent<SwapRotationFromHandedness>();
        instance.leftHandedRotation = leftHandedRotation;
        instance.rightHandedRotation = rightHandedRotation;
    }

    protected override void HandednessChanged()
    {
        transform.localRotation = VrSettings.LeftHandedMode.Value ? leftHandedRotation : rightHandedRotation;
    }
}