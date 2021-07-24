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

        public static VRHand Create(Transform parent,  bool isLeft = false)
        {
            var handName = isLeft ? "Left" : "Right";
            MelonLogger.Msg("## creating hand " + handName);
            var transform = parent.Find($"{handName}Hand");
            var instance = transform.gameObject.AddComponent<VRHand>();
            instance.handName = handName;
            instance.isLeft = isLeft;
            instance.SetUpPose();
            return instance;
        }

        public void SetUp(Transform rootBone)
        {
            gameObject.SetActive(false);
            this.rootBone = rootBone;
            if (rootBone)
            {
                SetFallbackHandActive(false);
                EnableAnimatedHand();
            }
            else
            {
                SetFallbackHandActive(true);
            }
            gameObject.SetActive(true);
        } 

        private void SetUpPose()
        {
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
        }

        private void Start()
        {
            // Reset the tracking. For some reason if I don't do this the hands will be frozen.
            gameObject.SetActive(true);
        }

        private void SetFallbackHandActive(bool active)
        {
            var handModel = transform.Find("HandModel");
            if (!handModel) return;
            handModel.gameObject.SetActive(active);
        }

        private void EnableAnimatedHand()
        {
            if (!rootBone)
            {
                return;
            }

            var armBone = SetUpArmBone();
            SetUpHandLid(armBone);
            var handBone = SetUpHandBone(armBone);
            MelonLogger.Msg("Pre VRMap.Create");
            // TODO create map
            //VRMap.Create(handBone, handName);
        }

        private Transform SetUpArmBone()
        {
            var armBone = rootBone.Find($"henryPelvis/henrySpineA/henrySpineB/henrySpineC/henrySpineD/henrySpider{handName}1/henrySpider{handName}2/henrySpider{handName}IK/henryArm{handName}Collarbone/henryArm{handName}1/henryArm{handName}2");
            var updateFollow = armBone.GetComponent<LateUpdateFollow>() ?? armBone.gameObject.AddComponent<LateUpdateFollow>();
            updateFollow.Target = transform.Find("ArmTarget");
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
            var wristTargetName = $"{handName}WristTarget";
            var wristTarget = armBone.Find(wristTargetName) ?? new GameObject(wristTargetName).transform;
            wristTarget.SetParent(armBone);
            var stabilizerAngleMultiplier = isLeft ? -1 : 1;
            wristTarget.localPosition = new Vector3(-0.2497151f, 0f, 0f);
            wristTarget.localEulerAngles = new Vector3(3.949f * stabilizerAngleMultiplier, 17.709f * stabilizerAngleMultiplier, 12.374f);
            var handBone = armBone.transform.Find($"henryArm{handName}Hand");
            var handBoneFollow = handBone.GetComponent<LateUpdateFollow>() ?? handBone.gameObject.AddComponent<LateUpdateFollow>();
            handBoneFollow.Target = wristTarget;
            return handBone;
        }
    }

    public class LateUpdateFollow : MonoBehaviour
    {
        public Transform Target;

        void LateUpdate()
        {
            if (!Target)
            {
                return;
            }
            transform.position = Target.position;
            transform.rotation = Target.rotation;
        }
    }
}
