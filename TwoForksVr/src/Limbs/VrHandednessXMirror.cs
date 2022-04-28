using UnityEngine;

namespace TwoForksVr.Limbs;

public class VrHandednessXMirror : SwapScaleFromHandedness
{
    public static void Create(Transform transform)
    {
        var instance = transform.gameObject.AddComponent<VrHandednessXMirror>();
        instance.rightHandedScale = transform.localScale;
        instance.leftHandedScale =
            new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void Update()
    {
        // Some things I needed to flip were being reset somewhere, and I didn't feel like fixing that.
        // So I'm just forcing the correct scale every frame.
        HandednessChanged();
    }
}