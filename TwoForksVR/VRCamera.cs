using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR;

namespace Raicuparta.TwoForksVR
{
    public class VRCamera: MonoBehaviour
    {
        private bool isInitialized;

        private void Start()
        {
            SetUpCamera();
            LimitVerticalRotation();
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
    }
}
