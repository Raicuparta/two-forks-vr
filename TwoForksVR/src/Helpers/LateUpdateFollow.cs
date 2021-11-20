using UnityEngine;

namespace TwoForksVr.Helpers
{
    // This component is useful when we need to simulate object parenting,
    // without actually changing the hierarchy.
    public class LateUpdateFollow : MonoBehaviour
    {
        public Transform Target;
        public Vector3 LocalPosition;

        private void LateUpdate()
        {
            if (!Target) return;
            transform.position = Target.position + LocalPosition;
            transform.rotation = Target.rotation;
        }
    }
}
