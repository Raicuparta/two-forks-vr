using UnityEngine;

namespace TwoForksVr.UI
{
    public class StaticUi : TwoForksVrBehavior
    {
        private const float offset = 0.8f;
        private static Transform cameraTransform;

        protected override void VeryLateUpdate()
        {
            if (!cameraTransform) return;
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