using UnityEngine;

namespace TwoForksVR.UI
{
    public class AttachToCamera : MonoBehaviour
    {
        private const float offset = 0.8f;

        private void LateUpdate()
        {
            // TODO optimize this, don't access Camera.main every frame.
            var mainCamera = Camera.main;

            if (mainCamera == null) return;

            var cameraTransform = mainCamera.transform;
            var targetPosition = cameraTransform.position;
            transform.position = targetPosition + cameraTransform.forward * offset;
            transform.LookAt(2 * transform.position - targetPosition);
        }
    }
}