using System;
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

        private Transform cameraTransform;
        private CharacterController characterController;
        private vgPlayerNavigationController navigationController;
        private Vector3 prevCameraPosition;
        private Transform henryTransform;

        public static RoomScaleBodyTransform Instance;

        private void Start()
        {
            Instance = this;
            prevForward = GetCameraForward();
            prevCameraPosition = cameraTransform.position;
            Camera.onPreCull += HandlePreCull;
        }

        private void OnDestroy()
        {
            Camera.onPreCull -= HandlePreCull;
        }

        public float rotationSpeed = 200f;

        private void Update()
        {
            if (!navigationController.onGround || !navigationController.enabled) return;
            characterController.transform.Rotate(Vector3.up, SteamVR_Actions._default.Rotate.axis.x * rotationSpeed * Time.deltaTime);
        }

        private void HandlePreCull(Camera cam)
        {
            UpdateRotation();
            UpdateRoomScalePosition();
            // VrStage.Instance.Recenter();
            FakeParenting.InvokeEvent();
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

        public void UpdateRoomScalePosition()
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

        // private void UpdateRotation()
        // {
        //     if (!navigationController.onGround || !navigationController.enabled)
        //     {
        //         henryTransform.localRotation = Quaternion.identity;
        //         return;
        //     };
        //
        //     var cameraForward = MathHelper.GetProjectedForward(cameraTransform);
        //     henryTransform.rotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        // }
        
        private Vector3 GetCameraForward()
        {
            return cameraTransform.parent.InverseTransformDirection(MathHelper.GetProjectedForward(cameraTransform));
        }

        private Vector3 prevForward;
        
        public void UpdateRotation()
        {
            if (!navigationController.onGround || !navigationController.enabled) return;
            var cameraForward = GetCameraForward();
            var angleDelta = MathHelper.SignedAngle(prevForward, cameraForward, Vector3.up);
            prevForward = cameraForward;
            characterController.transform.Rotate(Vector3.up, angleDelta);
            
            VrStage.Instance.Recenter();
            // VrStage.Instance.transform.RotateAround(characterController.transform.position, Vector3.up, -angleDelta);
        }
    }
}