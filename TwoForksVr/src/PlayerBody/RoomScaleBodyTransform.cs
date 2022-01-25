using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.PlayerBody
{
    public class RoomScaleBodyTransform : TwoForksVrBehavior
    {
        private const float minPositionOffset = 0.00001f;
        private const float maxPositionOffset = 1f;

        private Transform cameraTransform;
        private CharacterController characterController;
        private vgPlayerNavigationController navigationController;
        private Vector3 prevCameraPosition;
        private Vector3 prevForward;

        public static void Create(vgPlayerController playerController)
        {
            var playerTransform = playerController.transform;
            var existingRoomScalePosition = playerTransform.gameObject.GetComponent<RoomScaleBodyTransform>();
            if (existingRoomScalePosition) return;

            var instance = playerController.gameObject.AddComponent<RoomScaleBodyTransform>();
            instance.cameraTransform = playerController.playerCamera.transform;
            instance.characterController = playerController.characterController;
            instance.navigationController = playerController.navController;
        }

        protected override void Awake()
        {
            base.Awake();
            SteamVR_Actions.default_Teleport.onStateDown += OnTeleportInput;
            SteamVR_Actions.default_Teleport.onStateUp += OnTeleportInput;
        }

        private void Start()
        {
            prevForward = GetCameraForward();
            prevCameraPosition = cameraTransform.position;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SteamVR_Actions.default_Teleport.onStateDown -= OnTeleportInput;
            SteamVR_Actions.default_Teleport.onStateUp -= OnTeleportInput;
        }

        protected override void VeryLateUpdate()
        {
            UpdateRotation();
            UpdateRoomScalePosition();
            Recenter();
        }

        private void Recenter()
        {
            if (!navigationController.onGround || !navigationController.enabled) return;
            VrStage.Instance.RecenterRotation();
            VrStage.Instance.RecenterPosition();
        }

        private void OnTeleportInput(SteamVR_Action_Boolean fromaction, SteamVR_Input_Sources fromsource)
        {
            // TODO this probably doesn't make sense, what if teleport option is turned off?
            enabled = !fromaction.state;
        }

        private void UpdateRoomScalePosition()
        {
            var cameraPosition = cameraTransform.localPosition;

            var localPositionDelta = cameraPosition - prevCameraPosition;
            localPositionDelta.y = 0;

            var worldPositionDelta = VrStage.Instance.transform.TransformVector(localPositionDelta);

            if (worldPositionDelta.sqrMagnitude < minPositionOffset || !navigationController.onGround ||
                !navigationController.enabled) return;

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
    }
}