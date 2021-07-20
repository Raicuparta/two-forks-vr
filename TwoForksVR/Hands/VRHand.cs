using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR;
using Valve.VR;
using TwoForksVR.Tools;

namespace TwoForksVR.Hands
{
    public class VRHand: MonoBehaviour
    {
        private bool isLeft;

        public static VRHand Create(Transform parent, bool isLeft = false)
        {
            var name = isLeft ? "Left" : "Right";
            var transform = parent.Find($"{name}Hand");
            var instance = transform.gameObject.AddComponent<VRHand>();
            instance.isLeft = isLeft;

            return instance;
        }

        private void Start()
        {
            gameObject.SetActive(false);
            name = $"{(isLeft ? "Left" : "Right")} Hand";
            var pose = gameObject.AddComponent<SteamVR_Behaviour_Pose>();

            if (isLeft)
            {
                pose.inputSource = SteamVR_Input_Sources.LeftHand;
                pose.poseAction = SteamVR_Actions.default_PoseLeftHand;
            }
            else
            {
                pose.inputSource = SteamVR_Input_Sources.RightHand;
                pose.poseAction = SteamVR_Actions.default_PoseRightHand;
            }
            gameObject.SetActive(true);
        }

        private void SetUpMap()
        {
            var mapInHand = transform.Find("itemSocket/henryHandLeftAttachment/MapRiggedPosedPrefab(Clone)/MapRoot/MapInHand");
            if (!mapInHand)
            {
                return;
            }
            mapInHand.gameObject.AddComponent<VRMap>();
        }
    }
}
