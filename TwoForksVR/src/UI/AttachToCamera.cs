using UnityEngine;

namespace TwoForksVr.UI
{
    public class AttachToCamera : MonoBehaviour
    {
        private const float offset = 0.8f;
        private static Transform cameraTransform;

        private void LateUpdate()
        {
            var targetPosition = cameraTransform.position;
            transform.position = targetPosition + cameraTransform.forward * offset;
            transform.LookAt(2 * transform.position - targetPosition);
        }

        public static void SetTargetCamera(Camera camera)
        {
            cameraTransform = camera ? camera.transform : null;
        }
    }
}
