using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using TwoForksVr.UI;
using UnityEngine;
using UnityEngine.XR;
using UnityStandardAssets.ImageEffects;
using Valve.VR;

namespace TwoForksVr.PlayerCamera
{
    public class VRCameraManager : MonoBehaviour
    {
        private Camera camera;
        private vgCameraController cameraController;
        private int cameraCullingMask;
        private VrStage stage;
        private Transform playerTransform;

        private void Update()
        {
            if (SteamVR_Actions.default_Recenter.stateDown) Recenter(true);
            UpdateCulling();
        }

        private void UpdateCulling()
        {
            if (!vgPauseManager.Instance) return;
            // TODO: maybe need to do this some other way? Some menus don't have UI layer.
            if (cameraCullingMask == 0 && vgPauseManager.Instance.isPaused)
            {
                cameraCullingMask = camera.cullingMask;
                camera.cullingMask = LayerHelper.GetMask(GameLayer.UI, GameLayer.MenuBackground, GameLayer.PlayerBody);
            }
            else if (cameraCullingMask != 0 && !vgPauseManager.Instance.isPaused)
            {
                camera.cullingMask = cameraCullingMask;
                cameraCullingMask = 0;
            }
        }

        public static VRCameraManager Create(VrStage stage)
        {
            var instance = stage.gameObject.AddComponent<VRCameraManager>();
            instance.stage = stage;
            return instance;
        }

        public void SetUp(Camera newCamera, Transform newPlayerTransform)
        {
            AttachToCamera.SetTargetCamera(newCamera);
            camera = newCamera;
            playerTransform = newPlayerTransform;
            cameraController = FindObjectOfType<vgCameraController>();
            SetUpCamera();
            DisableCameraComponents();
            // Recenter camera after a while. Just in case it didn't work the first time.
            Invoke(nameof(RecenterIncludingVertical), 1);
        }

        private void RecenterIncludingVertical()
        {
            Recenter(true);
        }

        private void SetUpCamera()
        {
            camera.nearClipPlane = 0.03f;
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
            cameraParent.gameObject.AddComponent<LateUpdateFollow>().Target = stage.transform;
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

        public void Recenter(bool recenterVertically = false)
        {
            if (!playerTransform || !camera) return;
            var cameraOffset = GetCameraOffset();
            if (!recenterVertically)
            {
                cameraOffset.y = 0;
            }
            transform.position -= cameraOffset;
            var angleOffset = playerTransform.eulerAngles.y - camera.transform.eulerAngles.y;
            transform.Rotate(Vector3.up * angleOffset);
        }
    }
}
