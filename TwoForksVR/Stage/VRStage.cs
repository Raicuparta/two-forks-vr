using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwoForksVR.Hands;
using TwoForksVR.PlayerCamera;
using UnityEngine;
using UnityEngine.VR;

namespace TwoForksVR.Stage
{
    class VRStage: MonoBehaviour
    {
        private Camera camera;

        public static VRStage Create()
        {
            var instance = new GameObject("VRStage").AddComponent<VRStage>();

            var camera = Camera.main;
            if (!camera)
            {
                //camera = new GameObject("VR Camera").AddComponent<Camera>();
                //camera.tag = "MainCamera";
                //camera.enabled = false;
                VRSettings.enabled = false;
                return instance;
            }
            instance.camera = camera;
            var transform = instance.transform;
            transform.SetParent(camera.transform.parent, false);
            VRCameraManager.Create(parent: transform);
            //VRHandsManager.Create(
            //    parent: transform,
            //    playerBody: VRBodyManager.GetPlayerBodyTransform()
            //);
            return instance;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.C))
            {
                camera.enabled = !camera.enabled;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.V))
            {
                VRSettings.enabled = !VRSettings.enabled;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
            {
                VRSettings.enabled = VRStage.Create();
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Equals))
            {
                Time.timeScale = Time.timeScale > 1 ? 1 : 10;
            }
        }
    }
}
