using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static VRCameraManager Create(VRStage stage)
        {
            var instance = stage.gameObject.AddComponent<VRCameraManager>();
            return instance;
        }

        public void SetUp(Camera camera)
        {
            this.camera = camera;
            StartTodo();
        }

        private void StartTodo()
        {
            cameraController = FindObjectOfType<vgCameraController>();
            //VRSettings.enabled = false; TODO
            SetUpCamera();
            LimitVerticalRotation();
            DisableCameraAnimations();
            // Recenter camera after a while. Hack, need to figure out when I can call it.
            Invoke(nameof(RecenterCamera), 1f);
        }

        private void Update()
        {
            if (SteamVR_Actions.default_Recenter.stateDown)
            {
                RecenterCamera();
            }
        }

        private void SetUpCamera()
        {
            camera.transform.SetParent(transform);
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.identity;
            camera.nearClipPlane = 0.03f;
            camera.depth = 1; // Make sure this camera draws on top of other cameras we make.
            if (!isInitialized)
            {
                VRSettings.enabled = true;
                isInitialized = true;
            }
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

        private void DisableCameraAnimations()
        {
            var animation = camera.GetComponent<Animation>();
            if (!animation) return;
            animation.enabled = false;
        }

        private Vector3 GetCameraOffset()
        {
            return camera.transform.position - cameraController.eyeTransform.position;
        }

        private void RecenterCamera()
        {
            if (!cameraController)
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
