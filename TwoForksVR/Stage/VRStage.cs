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
            VRStage instance;
            var parent = camera?.transform.parent;

            if (!parent)
            {
                var existingStage = GameObject.Find("VRStage")?.GetComponent<VRStage>();
                if (existingStage) return existingStage;
            }

            var stageParent = new GameObject("VRStageParent").transform;
            var stageTransform = new GameObject("VRStage").transform;
            stageTransform.SetParent(stageParent, false);
            instance = stageTransform.gameObject.AddComponent<VRStage>();

            if (camera)
            {
                stageParent.gameObject.AddComponent<LateUpdateFollow>().Target = parent;
            } 
            else
            {
                camera = new GameObject("VR Camera").AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.Color;
                camera.backgroundColor = Color.black;
                camera.tag = "MainCamera";
                IntroFix.Create();
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
            if (UnityEngine.Input.GetKeyDown(KeyCode.Equals))
            {
                Time.timeScale = Time.timeScale > 1 ? 1 : 10;
            }
        }
    }
}
