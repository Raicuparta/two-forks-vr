using TwoForksVr.Helpers;
using TwoForksVr.Locomotion;
using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.PlayerBody
{
    public class RoomScaleBodyTransform : TwoForksVrBehavior
    {
        private const float minPositionOffset = 0;
        private const float maxPositionOffset = 1f;

        private Transform cameraTransform;
        private CharacterController characterController;
        private vgPlayerNavigationController navigationController;
        private Vector3 prevCameraPosition;
        private Vector3 prevForward;
        private bool previousNavigationControlerEnabled;
        private VrStage stage;
        private TeleportController teleportController;

        public static RoomScaleBodyTransform Create(VrStage stage, TeleportController teleportController)
        {
            var instance = stage.gameObject.AddComponent<RoomScaleBodyTransform>();
            instance.teleportController = teleportController;
            instance.stage = stage;
            return instance;
        }

        public void SetUp(vgPlayerController playerController)
        {
            if (!playerController) return;
            cameraTransform = playerController.playerCamera.transform;
            characterController = playerController.characterController;
            navigationController = playerController.navController;
            prevCameraPosition = cameraTransform.position;
            prevForward = GetCameraForward();
        }

        private void Update()
        {
            UpdateRecenterOnEnablingNavigationController();
        }

        protected override void VeryLateUpdate()
        {
            if (ShouldSkipUpdate()) return;
            UpdateRotation();
            UpdateRoomScalePosition();
        }

        // The navigation controller gets disabled while some larger animations are playing.
        // Room scale body transform adjustments are paused while the navigation controller is disabled.
        // So we recenter the position and rotation when the navigation controller is enabled again.
        private void UpdateRecenterOnEnablingNavigationController()
        {
            if (!navigationController) return;

            if (navigationController.enabled && !previousNavigationControlerEnabled)
            {
                stage.RecenterPosition(true);
                stage.RecenterRotation();
            }

            previousNavigationControlerEnabled = navigationController.enabled;
        }

        private bool ShouldSkipUpdate()
        {
            return !characterController || teleportController && teleportController.IsTeleporting();
        }

        private void UpdateRoomScalePosition()
        {
            var cameraPosition = cameraTransform.localPosition;

            var localPositionDelta = cameraPosition - prevCameraPosition;
            localPositionDelta.y = 0;

            var worldPositionDelta = stage.transform.TransformVector(localPositionDelta);

            if (worldPositionDelta.sqrMagnitude < minPositionOffset || !navigationController.onGround ||
                !navigationController.enabled) return;

            prevCameraPosition = cameraPosition;

            if (worldPositionDelta.sqrMagnitude > maxPositionOffset) return;

            var groundedPositionDelta = Vector3.ProjectOnPlane(worldPositionDelta, navigationController.groundNormal);

            characterController.Move(groundedPositionDelta);

            // There's a chance this might break some other movement-related stuff,
            // like resetting animations.
            navigationController.positionLastFrame = characterController.transform.position;

            stage.RecenterPosition();
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

            stage.RecenterRotation();
        }
    }
}