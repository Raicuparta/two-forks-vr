using UnityEngine;

namespace TwoForksVr.Helpers;

public class CopyLocalTransformValues : TwoForksVrBehavior
{
    private Transform target;

    public static void Create(GameObject gameObject, Transform target)
    {
        var instance = gameObject.GetComponent<CopyLocalTransformValues>();
        if (!instance) instance = gameObject.AddComponent<CopyLocalTransformValues>();

        instance.target = target;
    }

    protected override void VeryLateUpdate()
    {
        if (!target)
        {
            Destroy(this);
            return;
        }

        transform.localRotation = target.localRotation;
        transform.localPosition = target.localPosition;
        transform.localScale = target.localScale;
    }
}