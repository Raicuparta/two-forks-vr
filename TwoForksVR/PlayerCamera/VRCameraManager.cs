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
        private vgCameraController cameraController;

        public static VRCameraManager Create(Transform parent)
        {
            var instance = parent.gameObject.AddComponent<VRCameraManager>();
            return instance;
        }

        private void Start()
        {
            Instance = this;
            cameraController = FindObjectOfType<vgCameraController>();
            VRSettings.enabled = false;
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
            var camera = Camera.main;
            camera.transform.SetParent(transform);
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
            transform.position -= cameraOffset;
            var angleOffset = cameraController.eyeTransform.eulerAngles.y - Camera.main.transform.eulerAngles.y - 90f;
            transform.Rotate(Vector3.up * angleOffset);
        }
    }
}
