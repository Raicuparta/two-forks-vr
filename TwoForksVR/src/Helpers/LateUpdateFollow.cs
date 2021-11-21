using System;
using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.Helpers
{
    // This component is useful when we need to simulate object parenting,
    // without actually changing the hierarchy.
    public class LateUpdateFollow : MonoBehaviour
    {
        public Transform Target;

        private void Awake()
        {
            Camera.onPreCull += UpdateTransform;
        }

        private void OnDestroy()
        {
            Camera.onPreCull -= UpdateTransform;
        }

        private void UpdateTransform(Camera camera)
        {
            if (!Target || camera != VRStage.Instance.MainCamera) return;
            transform.position = Target.position;
            transform.rotation = Target.rotation;
        }
    }
}
