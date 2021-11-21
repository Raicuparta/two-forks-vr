using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using TwoForksVr.UI;
using UnityEngine;
using UnityEngine.VR;
using UnityStandardAssets.ImageEffects;
using Valve.VR;

namespace TwoForksVr.PlayerCamera
{
    public class VRCameraManager : MonoBehaviour
    {
        private Camera camera;
        private vgCameraController cameraController;
        private int cameraCullingMask;
        private VRStage stage;

        private void Update()
        {
            if (SteamVR_Actions.default_Recenter.stateDown) Recenter();
            UpdateCulling();
        }

        private void UpdateCulling()
        {
            if (!vgPauseManager.Instance) return;
            // TODO: maybe need to do this some other way? Some menus don't have UI layer.
            if (cameraCullingMask == 0 && vgPauseManager.Instance.isPaused)
            {
                cameraCullingMask = camera.cullingMask;
                camera.cullingMask = LayerMask.GetMask("UI", "MenuBackground");
            }
            else if (cameraCullingMask != 0 && !vgPauseManager.Instance.isPaused)
            {
                camera.cullingMask = cameraCullingMask;
                cameraCullingMask = 0;
            }
        }

        public static VRCameraManager Create(VRStage stage)
        {
            var instance = stage.gameObject.AddComponent<VRCameraManager>();
            instance.stage = stage;
            return instance;
        }

        public void SetUp(Camera newCamera)
        {
            AttachToCamera.SetTargetCamera(newCamera);
            camera = newCamera;
            cameraController = FindObjectOfType<vgCameraController>();
            SetUpCamera();
            LimitVerticalRotation();
            DisableCameraComponents();
            // Recenter camera after a while. Just in case it didn't work the first time.
            Invoke(nameof(Recenter), 1);
        }

        private void SetUpCamera()
        {
            camera.nearClipPlane = 0.03f;
            var cameraTransform = camera.transform;

            if (cameraTransform.parent && cameraTransform.parent.name == "VRCameraParent") return;

            // Probably an old Unity bug: if the camera starts with an offset position,
            // the tracking will always be incorrect.
            // So I disable VR, reset the camera position, and then enable VR again.
            VRSettings.enabled = false;
            
            var cameraParent = new GameObject("VRCameraParent").transform;
            cameraParent.SetParent(cameraTransform.parent, false);
            cameraTransform.SetParent(cameraParent.transform);
            cameraTransform.localPosition = Vector3.zero;
            cameraTransform.localRotation = Quaternion.identity;
            cameraParent.gameObject.AddComponent<FollowTarget>().Target = stage.transform;
            VRSettings.enabled = true;
        }

        private static void LimitVerticalRotation()
        {
            var cameraController = FindObjectOfType<vgCameraController>();
            if (!cameraController) return;
            cameraController.defaultCameraTuning.ForEach(tuning =>
            {
                tuning.minVerticalAngle = 0;
                tuning.maxVerticalAngle = 0;
            });
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

        public void Recenter()
        {
            if (!cameraController || !cameraController.eyeTransform) return;
            var cameraOffset = GetCameraOffset();
            transform.position -= cameraOffset;
            var angleOffset = cameraController.eyeTransform.eulerAngles.y - camera.transform.eulerAngles.y - 90f;
            transform.Rotate(Vector3.up * angleOffset);
        }
    }
}
