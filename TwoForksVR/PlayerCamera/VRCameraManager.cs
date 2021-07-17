using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR;
using Valve.VR;

namespace TwoForksVR.PlayerCamera
{
    public class VRCameraManager: MonoBehaviour
    {
        public static VRCameraManager Instance;

        private bool isInitialized;
        private Transform stage;
        private vgCameraController cameraController;

        private void Start()
        {
            cameraController = FindObjectOfType<vgCameraController>();
            Instance = this;
            VRSettings.enabled = false;
            SetUpCamera();
            LimitVerticalRotation();
            SetUpStage();
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
            var camera = Camera.main;
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.identity;
            camera.nearClipPlane = 0.03f;
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

        private Transform SetUpStage()
        {
            stage = new GameObject("VRStage").transform;
            stage.SetParent(Camera.main.transform.parent, false);
            Camera.main.transform.SetParent(stage);
            return stage;
        }

        private void DisableCameraAnimations()
        {
            var animation = Camera.main.GetComponent<Animation>();
            if (!animation) return;
            animation.enabled = false;
        }

        private Vector3 GetCameraOffset()
        {
            return Camera.main.transform.position - cameraController.eyeTransform.position;
        }

        private void RecenterCamera()
        {
            if (!cameraController)
            {
                return;
            }
            var cameraOffset = GetCameraOffset();
            stage.position -= cameraOffset;
            var angleOffset = cameraController.eyeTransform.eulerAngles.y - Camera.main.transform.eulerAngles.y - 90f;
            stage.Rotate(Vector3.up * angleOffset);
        }
    }
}
