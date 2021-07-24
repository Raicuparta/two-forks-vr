using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwoForksVR.Hands;
using TwoForksVR.Stage;
using UnityEngine;
using UnityEngine.VR;
using Valve.VR;

namespace TwoForksVR.PlayerCamera
{
    public class VRCameraManager: MonoBehaviour
    {
        private bool isInitialized;
        private vgCameraController cameraController;
        private Camera camera;
        private VRStage stage;

        public static VRCameraManager Create(VRStage stage)
        {
            var instance = stage.gameObject.AddComponent<VRCameraManager>();
            instance.stage = stage;
            return instance;
        }

        public void SetUp(Camera camera)
        {
            this.camera = camera;
            cameraController = FindObjectOfType<vgCameraController>();
            SetUpCamera();
            LimitVerticalRotation();
            DisableCameraComponents();
            // Recenter camera after a while. Just in case it didn't work the first time.
            Invoke(nameof(Recenter), 1);
        }

        private void Update()
        {
            if (SteamVR_Actions.default_Recenter.stateDown)
            {
                Recenter();
            }
        }

        private void SetUpCamera()
        {
            if (camera.transform.parent?.name != "VRCameraParent")
            {
                // Probably an old Unity bug: if the camera starts with an offset position,
                // the tracking will always be incorrect.
                // So I disable VR, reset the camera position, and then enable VR again.
                VRSettings.enabled = false;
                var cameraParent = new GameObject("VRCameraParent").transform;
                cameraParent.SetParent(camera.transform.parent, false);
                camera.transform.SetParent(cameraParent.transform);
                camera.transform.localPosition = Vector3.zero;
                camera.transform.localRotation = Quaternion.identity;
                cameraParent.gameObject.AddComponent<LateUpdateFollow>().Target = stage.transform;
                VRSettings.enabled = true;
            }
            camera.nearClipPlane = 0.03f;
            camera.depth = 1; // Make sure this camera draws on top of other cameras we make. TODO remove?
        }

        private void LimitVerticalRotation()
        {
            var cameraController = GameObject.FindObjectOfType<vgCameraController>();
            if (!cameraController)
            {
                return;
            }
            cameraController.defaultCameraTuning.ForEach(tuning => {
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
            DisableCameraComponent<UnityStandardAssets.ImageEffects.Bloom>();
            DisableCameraComponent<UnityStandardAssets.ImageEffects.Antialiasing>();
        }

        private void DisableCameraComponent<TComponent>() where TComponent: Behaviour
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
            if (!cameraController?.eyeTransform)
            {
                return;
            }
            var cameraOffset = GetCameraOffset();
            transform.position -= cameraOffset;
            var angleOffset = cameraController.eyeTransform.eulerAngles.y - camera.transform.eulerAngles.y - 90f;
            transform.Rotate(Vector3.up * angleOffset);
        }
    }
}
