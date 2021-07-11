using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR;

namespace TwoForksVR.PlayerCamera
{
    public class VRCameraManager: MonoBehaviour
    {
        public static VRCameraManager Instance;
        public Camera VRCamera;

        private bool isInitialized;
        
        private void Start()
        {
            Instance = this;
            VRSettings.enabled = false;
            SetUpCamera();
            LimitVerticalRotation();
            ReparentCamera();
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

        private void ReparentCamera()
        {
            var originalCamera = Camera.main;
            VRCamera = new GameObject("VRCamera").AddComponent<Camera>();
            VRCamera.CopyFrom(originalCamera);
            originalCamera.tag = "";
            originalCamera.enabled = false;
            VRCamera.tag = "MainCamera";
            var vrCameraParent = new GameObject("VRStage").transform;
            vrCameraParent.SetParent(originalCamera.transform.parent, false);
            VRCamera.transform.SetParent(vrCameraParent);
        }
    }
}
