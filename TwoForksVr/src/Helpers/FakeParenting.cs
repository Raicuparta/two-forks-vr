using UnityEngine;

namespace TwoForksVr.Helpers
{
    // This component is useful when we need to simulate object parenting,
    // without actually changing the hierarchy.
    public class FakeParenting : TwoForksVrBehavior
    {
        public Transform Target;

        public override void VeryLateUpdate()
        {
            if (!Target) return;
            transform.position = Target.position;
            transform.rotation = Target.rotation;
        }
    }
}