using Harmony;
using MelonLoader;
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
        public static VRStage Create(Camera camera, Transform playerTransform)
        {
            var parent = camera.transform.parent;
            var stageTransform = parent.Find("VRStage") ?? new GameObject("VRStage").transform;
            if (stageTransform.GetComponent<VRStage>()) return null;
            stageTransform.SetParent(parent, false);
            var instance = stageTransform.gameObject.AddComponent<VRStage>();

            if (!camera)
            {
                camera = new GameObject("VR Camera").AddComponent<Camera>();
                camera.tag = "MainCamera";
                return instance;
            }

            VRCameraManager.Create(
                parent: stageTransform,
                camera: camera
            );
            VRHandsManager.Create(
                parent: stageTransform,
                playerTransform: playerTransform
            );
            return instance;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.V))
            {
                VRSettings.enabled = !VRSettings.enabled;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
            {
                GameObject.Find("IntroManager").SetActive(false);
                GameObject.Find("IntroTextAndBackground").SetActive(false);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Equals))
            {
                Time.timeScale = Time.timeScale > 1 ? 1 : 10;
            }
        }
    }
}
