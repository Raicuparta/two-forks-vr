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
        public Transform PlayerBody; // TODO get this some other way.

        public Transform LeftHand;
        public Transform RightHand;
        //public Transform LeftHandAttachment;
        //public Transform RightHandAttachment;

        private void Start()
        {
            Instance = this;

            SetUpHands();
            SetUpToolPicker();
            SetUpHandLaser();
        }

        private void SetUpHands()
        {
            var hands = Instantiate(VRAssetLoader.Hands).transform;
            hands.SetParent(transform, false);

            RightHand = CreateHand(hands.Find("RightHand").gameObject);
            LeftHand = CreateHand(hands.Find("LeftHand").gameObject, true);

            var bodyRoot = PlayerBody.parent.Find("henryroot");
            DoHandShit(RightHand, bodyRoot);
            DoHandShit(LeftHand, bodyRoot, true);
        }

        private void SetUpToolPicker()
        {
            var toolPickerPrefab = VRAssetLoader.ToolPicker;

            var toolPicker = Instantiate(toolPickerPrefab).AddComponent<ToolPicker>();
            toolPicker.ParentWhileActive = Camera.main.transform.parent;
            toolPicker.ParentWhileInactive = toolPicker.transform;
            toolPicker.ToolsContainer = toolPicker.transform.Find("Tools");

            foreach (Transform child in toolPicker.ToolsContainer)
            {
                var toolPickerItem = child.gameObject.AddComponent<ToolPickerItem>();
                toolPickerItem.ItemType = (ToolPicker.VRToolItem)Enum.Parse(typeof(ToolPicker.VRToolItem), child.name);
            }
        }

        private void SetUpHandLaser()
        {
            var handLaser = new GameObject("VRHandLaser").AddComponent<VRHandLaser>().transform;
            handLaser.SetParent(transform, false);
        }

        private Transform CreateHand(GameObject instance, bool isLeft = false)
        {
            var hand = instance.AddComponent<VRHand>();
            hand.IsLeft = isLeft;

            return hand.transform;
        }

        private Transform SetUpHandAttachment(Transform hand, string handName, Vector3 position, Vector3 eulerAngles)
        {
            var itemSocket = hand.Find("itemSocket");
            var handAttachment = GameObject.Find($"henryHand{handName}Attachment").transform;
            handAttachment.SetParent(itemSocket, false);
            itemSocket.localPosition = position;
            itemSocket.localEulerAngles = eulerAngles;
            return handAttachment;
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
        private static readonly Vector3 scale = Vector3.one * 0.8f;

        void LateUpdate()
        {
            transform.position = Target.position;
            transform.rotation = Target.rotation;
            //transform.localScale = scale;
        }
    }
}
