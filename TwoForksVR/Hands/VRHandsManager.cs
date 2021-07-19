using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TwoForksVR.Tools;
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
        public Transform LeftHandAttachment;
        public Transform RightHandAttachment;

        private void Start()
        {
            Instance = this;

            SetUpHands();
            SetUpToolPicker();
            SetUpHandLaser();

            DoHandShit(PlayerBody.parent.Find("henryroot"));
        }

        private void SetUpHands()
        {
            var handPrefab = VRAssetLoader.Hand;

            var handMaterial = GetHandMaterial();
            RightHand = CreateHand(handPrefab, handMaterial);
            LeftHand = CreateHand(handPrefab, handMaterial, true);
            LeftHandAttachment = SetUpHandAttachment(
                LeftHand,
                "Left",
                new Vector3(0.0157f, -0.0703f, -0.0755f),
                new Vector3(8.3794f, 341.5249f, 179.2709f)
            );
            RightHandAttachment = SetUpHandAttachment(
                RightHand,
                "Right",
                new Vector3(0.0551f, -0.0229f, -0.131f),
                new Vector3(54.1782f, 224.7767f, 139.0415f)
            );

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

        private void DoHandShit(Transform rootTransform)
        {
            var rightHandBone = rootTransform
                .Find("henryPelvis/henrySpineA/henrySpineB/henrySpineC/henrySpineD/henrySpiderRight1/henrySpiderRight2/henrySpiderRightIK/henryArmRightCollarbone/henryArmRight1")
                .gameObject;
            rightHandBone.AddComponent<LateUpdateFollow>().Target = RightHand;
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
