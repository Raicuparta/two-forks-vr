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
        public bool IsLeft = false;

        private void Start()
        {
            gameObject.SetActive(false);
            name = $"{(IsLeft ? "Left" : "Right")} Hand";
            //transform.SetParent(Camera.main.transform.parent, false); // TODO make sure camera is initialized?
            var pose = gameObject.AddComponent<SteamVR_Behaviour_Pose>();

            //var handModel = transform.Find("handModel");
            //handModel.gameObject.SetActive(false);
            if (IsLeft)
            {
                //handModel.localScale = new Vector3(-handModel.localScale.x, handModel.localScale.y, handModel.localScale.z);
                //SetUpWeddingRing();
                //SetUpMap();
                //SetUpRadio();
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
