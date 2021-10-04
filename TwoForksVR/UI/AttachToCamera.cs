using UnityEngine;

namespace TwoForksVR.UI
{
    public class AttachToCamera : MonoBehaviour
    {
        private static Transform cameraTransform;
        private const float offset = 0.8f;
        
        public static void SetTargetCamera(Camera camera)
        {
            cameraTransform = camera ? camera.transform : null;
        }
        
        private void LateUpdate()
        {
            var targetPosition = cameraTransform.position;
            transform.position = targetPosition + cameraTransform.forward * offset;
            transform.LookAt(2 * transform.position - targetPosition);
        }
    }
}