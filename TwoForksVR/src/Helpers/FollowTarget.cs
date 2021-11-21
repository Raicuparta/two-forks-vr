using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.Helpers
{
    // This component is useful when we need to simulate object parenting,
    // without actually changing the hierarchy.
    public class FollowTarget : MonoBehaviour
    {
        public Transform Target;
        public UpdateType TransformUpdateType = UpdateType.PreCull;

        private void Awake()
        {
            if (TransformUpdateType == UpdateType.PreCull)
            {
                Camera.onPreCull += HandlePreCull;
            }
        }

        private void LateUpdate()
        {
            if (TransformUpdateType != UpdateType.LateUpdate) return;
            UpdateTransform();
        }

        private void HandlePreCull(Camera camera)
        {
            if (camera != VRStage.Instance.MainCamera) return;
            UpdateTransform();
        }

        private void OnDestroy()
        {
            if (TransformUpdateType == UpdateType.PreCull)
            {
                Camera.onPreCull -= HandlePreCull;
            }
        }

        private void UpdateTransform()
        {
            if (!Target) return;
            transform.position = Target.position;
            transform.rotation = Target.rotation;
        }
    }

    public enum UpdateType
    {
        LateUpdate,
        PreCull,
    }
}
