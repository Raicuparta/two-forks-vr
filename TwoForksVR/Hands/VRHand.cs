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

        public static VRHand Create(Transform parent, Transform rootBone, bool isLeft = false)
        {
            var name = isLeft ? "Left" : "Right";
            var transform = parent.Find($"{name}Hand");
            var instance = transform.gameObject.AddComponent<VRHand>();
            instance.isLeft = isLeft;
            instance.rootBone = rootBone;
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
            DoHandShit();
        }

        private void DoHandShit()
        {
            if (!rootBone)
            {
                return;
            }

            var name = isLeft ? "Left" : "Right";

            var armBone = rootBone.Find($"henryPelvis/henrySpineA/henrySpineB/henrySpineC/henrySpineD/henrySpider{name}1/henrySpider{name}2/henrySpider{name}IK/henryArm{name}Collarbone/henryArm{name}1/henryArm{name}2");
            armBone.gameObject.AddComponent<LateUpdateFollow>().Target = transform.Find("ArmTarget");

            var handLid = Instantiate(VRAssetLoader.HandLid).transform;
            handLid.SetParent(armBone, false);
            if (isLeft)
            {
                handLid.localScale = new Vector3(1, 1, -1);
            }

            var stabilizerTarget = new GameObject($"{name}HandStabilizerTarget").transform;
            stabilizerTarget.SetParent(armBone);

            var stabilizerAngleMultiplier = isLeft ? -1 : 1;

            stabilizerTarget.localPosition = new Vector3(-0.2497151f, 0f, 0f);
            stabilizerTarget.localEulerAngles = new Vector3(3.949f * stabilizerAngleMultiplier, 17.709f * stabilizerAngleMultiplier, 12.374f);

            var handBone = armBone.transform.Find($"henryArm{name}Hand").gameObject;
            handBone.AddComponent<LateUpdateFollow>().Target = stabilizerTarget;
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

    public class LateUpdateFollow : MonoBehaviour
    {
        public Transform Target;
        private static readonly Vector3 scale = Vector3.one * 0.925f;

        void LateUpdate()
        {
            transform.position = Target.position;
            transform.rotation = Target.rotation;
            transform.localScale = scale;
        }
    }
}
