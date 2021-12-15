using UnityEngine;

namespace TwoForksVr.UI
{
    public class AttachMenuToCamera : AttachToCamera
    {
        private const float offset = 3f;

        protected override void HandleTargetCameraSet()
        {
            UpdateTransform();
        }

        private void OnEnable()
        {
            UpdateTransform();
        }

        private void Start()
        {
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (!CameraTransform) return;
            var targetPosition = CameraTransform.position;
            var forward = Vector3.ProjectOnPlane(CameraTransform.forward, Vector3.up);
            transform.position = targetPosition + forward * offset;
            transform.LookAt(2 * transform.position - targetPosition);
        }
    }
}
