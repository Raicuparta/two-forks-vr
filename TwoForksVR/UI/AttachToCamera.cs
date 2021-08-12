using UnityEngine;
using UnityExplorer;

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

            var targetPosition = mainCamera.transform.position;
            transform.position = targetPosition + mainCamera.transform.forward * offset;
            transform.LookAt(2 * transform.position - targetPosition);
        }
    }
}