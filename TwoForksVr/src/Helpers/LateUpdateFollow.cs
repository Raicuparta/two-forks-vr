using UnityEngine;

namespace TwoForksVr.Helpers
{
    // This component is useful when we need to simulate object parenting,
    // without actually changing the hierarchy.
    public class LateUpdateFollow : MonoBehaviour
    {
        public Transform Target;

        private void LateUpdate()
        {
            if (!Target) return;
            transform.position = Target.position;
            transform.rotation = Target.rotation;
        }
    }
}