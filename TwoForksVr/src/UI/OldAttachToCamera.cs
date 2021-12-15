using UnityEngine;

namespace TwoForksVr.UI
{
    public class OldAttachToCamera : MonoBehaviour
    {
        private const float offset = 0.8f;

        private void LateUpdate()
        {
            if (!AttachToCamera.cameraTransform) return;
            var targetPosition = AttachToCamera.cameraTransform.position;
            transform.position = targetPosition + AttachToCamera.cameraTransform.forward * offset;
            transform.LookAt(2 * transform.position - targetPosition);
        }
    }
}