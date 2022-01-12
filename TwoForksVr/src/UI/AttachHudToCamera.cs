namespace TwoForksVr.UI
{
    public class AttachHudToCamera : AttachToCamera
    {
        private const float offset = 0.8f;

        private void LateUpdate()
        {
            if (!CameraTransform) return;
            var targetPosition = CameraTransform.position;
            transform.position = targetPosition + CameraTransform.forward * offset;
            transform.LookAt(2 * transform.position - targetPosition);
        }

        protected override void HandleTargetCameraSet() {}
    }
}