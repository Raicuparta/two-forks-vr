using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TwoForksVR.Tools;
using TwoForksVR.Assets;
using UnityEngine;
using Valve.VR;

namespace TwoForksVR.Hands
{
    public class VRHandsManager: MonoBehaviour
    {
        public static VRHandsManager Instance;

        public Transform LeftHand;
        public Transform RightHand;

        private Transform playerBody;

        public static VRHandsManager Create(Transform parent, Transform playerBody)
        {
            var instance = Instantiate(VRAssetLoader.Hands).AddComponent<VRHandsManager>();
            instance.playerBody = playerBody;
            instance.transform.SetParent(parent, false);
            return instance;
        }

        private void Start()
        {
            Instance = this;

            SetUpHands();
            ToolPicker.Create(transform);
            VRHandLaser.Create(transform);
        }

        private void SetUpHands()
        {
            RightHand = CreateHand(transform.Find("RightHand").gameObject);
            LeftHand = CreateHand(transform.Find("LeftHand").gameObject, true);

            var bodyRoot = playerBody.parent.Find("henryroot");
            DoHandShit(RightHand, bodyRoot);
            DoHandShit(LeftHand, bodyRoot, true);
        }

        private Transform CreateHand(GameObject instance, bool isLeft = false)
        {
            var hand = instance.AddComponent<VRHand>();
            hand.IsLeft = isLeft;

            return hand.transform;
        }

        private void DoHandShit(Transform hand, Transform rootTransform, bool isLeft = false)
        {
            var name = isLeft ? "Left" : "Right";

            var armBone = rootTransform.Find($"henryPelvis/henrySpineA/henrySpineB/henrySpineC/henrySpineD/henrySpider{name}1/henrySpider{name}2/henrySpider{name}IK/henryArm{name}Collarbone/henryArm{name}1/henryArm{name}2");
            armBone.gameObject.AddComponent<LateUpdateFollow>().Target = hand.Find("ArmTarget");

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
