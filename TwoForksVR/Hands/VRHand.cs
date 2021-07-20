using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR;
using Valve.VR;
using TwoForksVR.Tools;
using TwoForksVR.Assets;

namespace TwoForksVR.Hands
{
    public class VRHand: MonoBehaviour
    {
        private bool isLeft;
        private Transform rootBone;
        private string handName;

        public static VRHand Create(Transform parent, Transform rootBone, bool isLeft = false)
        {
            var handName = isLeft ? "Left" : "Right";
            var transform = parent.Find($"{handName}Hand");
            var instance = transform.gameObject.AddComponent<VRHand>();
            instance.handName = handName;
            instance.isLeft = isLeft;
            instance.rootBone = rootBone;
            return instance;
        }

        private void Start()
        {
            gameObject.SetActive(false);
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
            MakeBonesFollowHands();
        }

        private void MakeBonesFollowHands()
        {
            if (!rootBone)
            {
                return;
            }

            var armBone = SetUpArmBone();
            SetUpHandLid(armBone);
            var handBone = SetUpHandBone(armBone);
            //SetUpMap(handBone);

        }

        private Transform SetUpArmBone()
        {
            var armBone = rootBone.Find($"henryPelvis/henrySpineA/henrySpineB/henrySpineC/henrySpineD/henrySpider{handName}1/henrySpider{handName}2/henrySpider{handName}IK/henryArm{handName}Collarbone/henryArm{handName}1/henryArm{handName}2");
            armBone.gameObject.AddComponent<LateUpdateFollow>().Target = transform.Find("ArmTarget");
            return armBone;
        }

        private void SetUpHandLid(Transform armBone)
        {
            var handLid = Instantiate(VRAssetLoader.HandLid).transform;
            handLid.SetParent(armBone, false);
            if (isLeft)
            {
                handLid.localScale = new Vector3(1, 1, -1);
            }
        }

        private Transform SetUpHandBone(Transform armBone)
        {
            var wristTarget = new GameObject($"{handName}WristTarget").transform;
            wristTarget.SetParent(armBone);
            var stabilizerAngleMultiplier = isLeft ? -1 : 1;
            wristTarget.localPosition = new Vector3(-0.2497151f, 0f, 0f);
            wristTarget.localEulerAngles = new Vector3(3.949f * stabilizerAngleMultiplier, 17.709f * stabilizerAngleMultiplier, 12.374f);
            var handBone = armBone.transform.Find($"henryArm{handName}Hand");
            handBone.gameObject.AddComponent<LateUpdateFollow>().Target = wristTarget;
            return handBone;
        }

        private void SetUpMap(Transform handBone)
        {
            var mapInHand = transform.Find("itemSocket/henryHandLeftAttachment/MapRiggedPosedPrefab(Clone)/MapRoot/MapInHand");
            if (!mapInHand)
            {
                return;
            }
            mapInHand.gameObject.AddComponent<VRMap>();
        }
    }

    public class LateUpdateFollow : MonoBehaviour
    {
        public Transform Target;
        public float scale = 1f;

        void LateUpdate()
        {
            transform.position = Target.position;
            transform.rotation = Target.rotation;
            transform.localScale = Vector3.one * scale;
        }
    }
}
