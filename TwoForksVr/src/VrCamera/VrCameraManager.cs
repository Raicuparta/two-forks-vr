using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using TwoForksVr.VrInput;
using UnityEngine;
using UnityEngine.XR;
using UnityStandardAssets.ImageEffects;

namespace TwoForksVr.VrCamera
{
    public class VRCameraManager : MonoBehaviour
    {
        private Camera camera;
        private Color cameraBackgroundColor;
        private vgCameraController cameraController;
        private int cameraCullingMask;
        private int pauseCameraCullingMask;
        private Transform playerTransform;
        private VrStage stage;

        public static VRCameraManager Create(VrStage stage)
        {
            var instance = stage.gameObject.AddComponent<VRCameraManager>();
            instance.stage = stage;
            return instance;
        }

        public void SetUp(Camera newCamera, Transform newPlayerTransform)
        {
            camera = newCamera;
            playerTransform = newPlayerTransform;
            cameraController = FindObjectOfType<vgCameraController>();
            SetUpCamera();
            DisableCameraComponents();
            // Recenter camera after a while. Just in case it didn't work the first time.
            Invoke(nameof(RecenterIncludingVertical), 1);
        }

        private void Start()
        {
            pauseCameraCullingMask = LayerHelper.GetMask(GameLayer.UI, GameLayer.MenuBackground, GameLayer.PlayerBody);
        }

        private void Update()
        {
            if (BindingsManager.ActionSet.Recenter.stateDown) RecenterPosition(true);
            UpdateCulling();
        }

        private void UpdateCulling()
        {
            if (!vgPauseManager.Instance) return;

            if (cameraCullingMask == 0 && vgPauseManager.Instance.isPaused)
            {
                cameraCullingMask = camera.cullingMask;
                cameraBackgroundColor = camera.backgroundColor;
                camera.cullingMask = pauseCameraCullingMask;
                camera.clearFlags = CameraClearFlags.Color;
                camera.backgroundColor = Color.black;
            }
            else if (cameraCullingMask != 0 && !vgPauseManager.Instance.isPaused)
            {
                camera.cullingMask = cameraCullingMask;
                cameraCullingMask = 0;
                camera.clearFlags = CameraClearFlags.Skybox;
                camera.backgroundColor = cameraBackgroundColor;
            }
        }

        private void RecenterIncludingVertical()
        {
            RecenterPosition(true);
        }

        private void SetUpCamera()
        {
            camera.nearClipPlane = 0.03f;
            camera.farClipPlane = 3000f;
            var cameraTransform = camera.transform;

            if (cameraTransform.parent && cameraTransform.parent.name == "VrCameraParent") return;

            // If the camera starts with an offset position, the tracking will always be incorrect.
            // So I disable VR, reset the camera position, and then enable VR again.
            XRSettings.enabled = false;

            var cameraParent = new GameObject("VrCameraParent").transform;
            cameraParent.SetParent(cameraTransform.parent, false);
            cameraTransform.SetParent(cameraParent.transform);
            cameraTransform.localPosition = Vector3.zero;
            cameraTransform.localRotation = Quaternion.identity;
            FakeParenting.Create(cameraParent, stage.transform);
            XRSettings.enabled = true;
        }

        private void DisableCameraComponents()
        {
            DisableCameraComponent<Animation>();
            DisableCameraComponent<AmplifyMotionCamera>();
            DisableCameraComponent<vgMenuCameraController>();
            DisableCameraComponent<AmplifyMotionEffect>();
            DisableCameraComponent<Bloom>();
            DisableCameraComponent<Antialiasing>();
        }

        private void DisableCameraComponent<TComponent>() where TComponent : Behaviour
        {
            var component = camera.GetComponent<TComponent>();
            if (!component) return;
            component.enabled = false;
        }

        private Vector3 GetCameraOffset()
        {
            try
            {
                return camera.transform.position - cameraController.eyeTransform.position;
            }
            catch
            {
                return Vector3.zero;
            }
        }

        public void RecenterPosition(bool recenterVertically = false)
        {
            if (!playerTransform || !camera) return;
            var cameraOffset = GetCameraOffset();
            if (!recenterVertically) cameraOffset.y = 0;
            transform.position -= cameraOffset;
        }

        public void RecenterRotation()
        {
            var angleOffset = playerTransform.eulerAngles.y - camera.transform.eulerAngles.y;
            transform.RotateAround(playerTransform.position, Vector3.up, angleOffset);
        }
    }
}