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

        private bool isInitialized;
        
        private void Start()
        {
            Instance = this;
            VRSettings.enabled = false;
            SetUpCamera();
            LimitVerticalRotation();
            ReparentCamera();
            DisableCameraAnimations();
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
            var vrCameraParent = new GameObject("VRStage").transform;
            vrCameraParent.SetParent(Camera.main.transform.parent, false);
            Camera.main.transform.SetParent(vrCameraParent);
        }

        private void DisableCameraAnimations()
        {
            var animation = Camera.main.GetComponent<Animation>();
            if (!animation) return;
            animation.enabled = false;
        }
    }
}
