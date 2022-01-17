using System;
using UnityEngine;

namespace TwoForksVr.Helpers
{
    // This component is useful when we need to simulate object parenting,
    // without actually changing the hierarchy.
    public class FakeParenting : MonoBehaviour
    {
        public Transform Target;

        private void Awake()
        {
            Camera.onPreCull += HandlePreCull;
        }

        private void OnDestroy()
        {
            Camera.onPreCull -= HandlePreCull;
        }

        private void HandlePreCull(Camera cam)
        {
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (!Target) return;
            transform.position = Target.position;
            transform.rotation = Target.rotation;
        }
    }
}