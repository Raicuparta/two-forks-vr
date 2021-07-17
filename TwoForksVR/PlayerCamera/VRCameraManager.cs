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
        }

        private void Update()
        {
            UpdateCameraOffset();
        }

        private void UpdateCameraOffset()
        {
            if (!cameraController)
            {
                return;
            }
            var cameraOffset = GetCameraOffset();
            if (SteamVR_Actions.default_Recenter.stateDown)
            {
                stage.position -= cameraOffset;
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

        public void MoveCameraToCorrectHeight(Vector3 offset)
        {
            //var eyeTransform = new GameObject("VREyeTransform").transform;

            stage.position -= offset;

            //eyeTransform.SetParent(cameraController.playerController.transform, false);
            //eyeTransform.position = cameraController.eyeTransform.position;
            //eyeTransform.rotation = cameraController.eyeTransform.rotation;
            //cameraController.eyeTransform = 
        }
    }
}
