using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwoForksVR.Hands;
using TwoForksVR.PlayerCamera;
using UnityEngine;

namespace TwoForksVR.Stage
{
    class VRStage: MonoBehaviour
    {
        public static VRStage Create()
        {
            var instance = new GameObject("VRStage").AddComponent<VRStage>();
            instance.transform.SetParent(Camera.main.transform.parent, false);
            return instance;
        }

        private void Start()
        {
            var playerBody = VRBodyManager.GetPlayerBodyTransform();
            VRHandsManager.Create(transform, playerBody);
            VRCameraManager.Create(transform);
        }
    }
}
