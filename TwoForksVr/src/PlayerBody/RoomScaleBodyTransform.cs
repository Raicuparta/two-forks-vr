using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.PlayerBody
{
    public class RoomScaleBodyTransform : MonoBehaviour
    {
        private const float minPositionOffset = 0.00001f;
        private const float maxPositionOffset = 1f;

        private Transform cameraTransform;
        private CharacterController characterController;
        private vgPlayerNavigationController navigationController;
        private Vector3 prevCameraPosition;
        private Transform henryTransform;

        private void Start()
        {
            prevCameraPosition = cameraTransform.position;
        }

        private void Update()
        {
            UpdateRoomScalePosition();
            UpdateRotation();
        }

        public static void Create(CharacterController characterController, Camera camera)
        {
            var playerTransform = characterController.transform;
            var existingRoomScalePosition = playerTransform.gameObject.GetComponent<RoomScaleBodyTransform>();
            if (existingRoomScalePosition) return;

            var instance = characterController.gameObject.AddComponent<RoomScaleBodyTransform>();
            instance.cameraTransform = camera.transform;
            instance.characterController = characterController;
            instance.henryTransform = characterController.transform.Find("henry");
            instance.navigationController = characterController.GetComponentInChildren<vgPlayerNavigationController>();
        }

        private void UpdateRoomScalePosition()
        {
            var cameraPosition = cameraTransform.localPosition;

            var localPositionDelta = cameraPosition - prevCameraPosition;
            localPositionDelta.y = 0;

            var worldPositionDelta = VrStage.Instance.transform.TransformVector(localPositionDelta);

            if (worldPositionDelta.sqrMagnitude < minPositionOffset || !navigationController.onGround || !navigationController.enabled) return;

            prevCameraPosition = cameraPosition;
            
            if (worldPositionDelta.sqrMagnitude > maxPositionOffset) return;

            var groundedPositionDelta = Vector3.ProjectOnPlane(worldPositionDelta, navigationController.groundNormal);

            characterController.Move(groundedPositionDelta);

            // TODO This probably breaks stuff elsewhere.
            navigationController.positionLastFrame = transform.position;

            VrStage.Instance.transform.position -= groundedPositionDelta;
        }

        private void UpdateRotation()
        {
            if (!navigationController.onGround || !navigationController.enabled)
            {
                henryTransform.localRotation = Quaternion.identity;
                return;
            };

            var cameraForward = MathHelper.GetProjectedForward(cameraTransform);
            henryTransform.rotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        }
    }
}