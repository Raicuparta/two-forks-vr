using System;
using UnityEngine;

namespace TwoForksVr.UI
{
    public abstract class StaticUi : MonoBehaviour
    {
        private static Transform cameraTransform;
        private const float offset = 0.8f;
        
        private void LateUpdate()
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
