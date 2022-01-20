using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.PlayerBody
{
    public class RoomScaleBodyTransform : MonoBehaviour
    {
        private const float minPositionOffset = 0.00001f;
        private const float maxPositionOffset = 1f;
        private const float rotationSpeed = 150f; // TODO make this configurable.

        private Transform cameraTransform;
        private CharacterController characterController;
        private vgPlayerNavigationController navigationController;
        private Vector3 prevCameraPosition;
        private Vector3 prevForward;
        public static RoomScaleBodyTransform Instance { get; private set; } // TODO remove after cleaning up FakeParenting.

        private void Start()
        {
            Instance = this;
            prevForward = GetCameraForward();
            prevCameraPosition = cameraTransform.position;
            Camera.onPreCull += HandlePreCull;
        }

        private void OnDestroy()
        {
            Instance = null;
            Camera.onPreCull -= HandlePreCull;
        }

        private void Update()
        {
            if (!navigationController.onGround || !navigationController.enabled) return;
            characterController.transform.Rotate(Vector3.up, SteamVR_Actions._default.Rotate.axis.x * rotationSpeed * Time.deltaTime);
        }

        private void HandlePreCull(Camera camera)
        {
            if (camera.transform != cameraTransform) return;

            UpdateRotation();
            UpdateRoomScalePosition();
            Recenter();
            FakeParenting.InvokeUpdate();
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
        }
        
        private Vector3 GetCameraForward()
        {
            return cameraTransform.parent.InverseTransformDirection(MathHelper.GetProjectedForward(cameraTransform));
        }
        
        private void UpdateRotation()
        {
            if (!navigationController.onGround || !navigationController.enabled) return;
            var cameraForward = GetCameraForward();
            var angleDelta = MathHelper.SignedAngle(prevForward, cameraForward, Vector3.up);
            prevForward = cameraForward;
            characterController.transform.Rotate(Vector3.up, angleDelta);
        }

        private void Recenter()
        {
            if (!navigationController.onGround || !navigationController.enabled) return;
            VrStage.Instance.Recenter();
        }
    }
}