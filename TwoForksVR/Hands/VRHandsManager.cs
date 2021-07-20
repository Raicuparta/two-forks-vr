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

            var bodyRoot = PlayerBody.parent.Find("henryroot");
            DoHandShit(bodyRoot);
            DoHandShit(bodyRoot, true);
        }

        private void SetUpHands()
        {
            var handPrefab = VRAssetLoader.Hand;

            var handMaterial = GetHandMaterial();
            RightHand = CreateHand(handPrefab, handMaterial);
            LeftHand = CreateHand(handPrefab, handMaterial, true);
            //LeftHandAttachment = SetUpHandAttachment(
            //    LeftHand,
            //    "Left",
            //    new Vector3(0.0157f, -0.0703f, -0.0755f),
            //    new Vector3(8.3794f, 341.5249f, 179.2709f)
            //);
            //RightHandAttachment = SetUpHandAttachment(
            //    RightHand,
            //    "Right",
            //    new Vector3(0.0551f, -0.0229f, -0.131f),
            //    new Vector3(54.1782f, 224.7767f, 139.0415f)
            //);

            // Update pickupAttachTransform to hand.
            GameObject.FindObjectOfType<vgInventoryController>().CachePlayerVariables();
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

        private Material GetHandMaterial()
        {
            if (!PlayerBody)
            {
                return null;
            }
            return PlayerBody.GetComponent<SkinnedMeshRenderer>().materials[2];
        }

        private Transform CreateHand(GameObject prefab, Material material, bool isLeft = false)
        {
            var hand = Instantiate(prefab).AddComponent<VRHand>();
            hand.IsLeft = isLeft;
            hand.SetMaterial(material);

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

        private void DoHandShit(Transform rootTransform, bool isLeft = false)
        {
            var name = isLeft ? "Left" : "Right";
            var trackingTarget = new GameObject($"{name}HandTarget").transform;
            trackingTarget.SetParent(isLeft ? LeftHand : RightHand, false);
            if (isLeft)
            {
                trackingTarget.localPosition = new Vector3(-0.00632076f, -0.04738592f, -0.3663795f);
                trackingTarget.localEulerAngles = new Vector3(-96.46899f, 22.21799f, 58.95399f);
            }
            else
            {
                trackingTarget.localPosition = new Vector3(0.01072523f, -0.04519948f, -0.3680739f);
                trackingTarget.localEulerAngles = new Vector3(84.35201f, -1.602f, -99.76801f);
            } 

            //target.localPosition = new Vector3(0.0707f, 0.0948f, -0.4881f);
            //target.localEulerAngles = new Vector3(347.1512f, 78.2487f, 43.7769f);

            var armBone = rootTransform.Find($"henryPelvis/henrySpineA/henrySpineB/henrySpineC/henrySpineD/henrySpider{name}1/henrySpider{name}2/henrySpider{name}IK/henryArm{name}Collarbone/henryArm{name}1/henryArm{name}2");
            armBone.gameObject.AddComponent<LateUpdateFollow>().Target = trackingTarget;

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

        void LateUpdate()
        {
            transform.position = Target.position;
            transform.rotation = Target.rotation;
        }
    }
}
