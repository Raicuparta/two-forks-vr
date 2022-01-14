using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.PlayerBody
{
    public class RoomScaleBodyTransform : MonoBehaviour
    {
        private Transform cameraTransform;
        private CharacterController characterController;
        private vgPlayerNavigationController navigationController;
        private Vector3 prevCameraPosition;
        private Vector3 prevForward;

        private void Start()
        {
            prevForward = GetCameraForward();
            prevCameraPosition = cameraTransform.position;
        }

        private void Update()
        {
            // UpdateRoomScalePosition();
            // UpdateRotation();
        }

        public static void Create(CharacterController characterController, Camera camera)
        {
            var playerTransform = characterController.transform;
            var existingRoomScalePosition = playerTransform.gameObject.GetComponent<RoomScaleBodyTransform>();
            if (existingRoomScalePosition) return;

            var instance = characterController.gameObject.AddComponent<RoomScaleBodyTransform>();
            instance.cameraTransform = camera.transform;
            instance.characterController = characterController;
            instance.navigationController = characterController.GetComponentInChildren<vgPlayerNavigationController>();
        }

        private Vector3 GetCameraForward()
        {
            return cameraTransform.parent.InverseTransformDirection(
                Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up));
        }

        private void UpdateRoomScalePosition()
        {
            var cameraPosition = cameraTransform.localPosition;

            var localPositionDelta = cameraPosition - prevCameraPosition;
            localPositionDelta.y = 0;

            var worldPositionDelta = VrStage.Instance.transform.TransformVector(localPositionDelta);

            prevCameraPosition = cameraPosition;

            if (worldPositionDelta.sqrMagnitude > 1f || !navigationController.onGround ||
                !navigationController.enabled) return;

            var groundedPositionDelta = Vector3.ProjectOnPlane(worldPositionDelta, navigationController.groundNormal);

            characterController.Move(groundedPositionDelta);

            // TODO This probably breaks stuff elsewhere.
            navigationController.positionLastFrame = transform.position;

            VrStage.Instance.transform.position -= groundedPositionDelta;
        }

        private void UpdateRotation()
        {
            if (!navigationController.onGround || !navigationController.enabled) return;

            var cameraForward = GetCameraForward();
            var angleDelta = MathHelper.SignedAngle(prevForward, cameraForward, Vector3.up);
            prevForward = cameraForward;
            characterController.transform.Rotate(Vector3.up, angleDelta);

            VrStage.Instance.Recenter();
        }
    }
}