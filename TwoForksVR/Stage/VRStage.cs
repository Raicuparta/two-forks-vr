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
            var transform = instance.transform;
            transform.SetParent(Camera.main.transform.parent, false);
            VRCameraManager.Create(parent: transform);
            VRHandsManager.Create(
                parent: transform,
                playerBody: VRBodyManager.GetPlayerBodyTransform()
            );
            return instance;
        }
    }
}
