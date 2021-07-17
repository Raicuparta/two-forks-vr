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
            transform.SetParent(Camera.main.transform.parent, false); // TODO make sure camera is initialized?
            var pose = gameObject.AddComponent<SteamVR_Behaviour_Pose>();

            if (IsLeft)
            {
                var handModel = transform.Find("handModel");
                handModel.localScale = new Vector3(-handModel.localScale.x, handModel.localScale.y, handModel.localScale.z);
                SetUpWeddingRing();
                SetUpMap();
                SetUpRadio();
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

        private void SetUpHandAttachment(Transform hand, string handName, Vector3 position, Vector3 eulerAngles)
        {
            var itemSocket = hand.Find("itemSocket");
            var handAttachment = GameObject.Find($"henryHand{handName}Attachment").transform;
            handAttachment.SetParent(itemSocket, false);
            itemSocket.localPosition = position;
            itemSocket.localEulerAngles = eulerAngles;
        }

        private void SetRightHandAttachment()
        {
            SetUpHandAttachment(
                transform,
                "Right",
                new Vector3(0.0551f, -0.0229f, -0.131f),
                new Vector3(54.1782f, 224.7767f, 139.0415f)
            );
        }

        private void SetLeftHandAttachment()
        {
            SetUpHandAttachment(
                transform,
                "Left",
                new Vector3(0.0157f, -0.0703f, -0.0755f),
                new Vector3(8.3794f, 341.5249f, 179.2709f)
            );
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

        private void SetUpRadio()
        {
            var radio = transform.Find("itemSocket/henryHandLeftAttachment/Radio(Clone)");
            if (!radio)
            {
                return;
            }
            radio.localPosition = new Vector3(-0.01f, -0.09f, 0.03f);
            radio.localEulerAngles = new Vector3(291.0985f, 133.8764f, 30.9742f);
        }

        private void SetUpWeddingRing()
        {
            var weddingRing = GameObject.Find("HenryWeddingRing 1")?.transform;
            if (!weddingRing)
            {
                return;
            }
            var socket = transform.Find("handModel/weddingRingSocket");
            weddingRing.SetParent(socket);
            weddingRing.localPosition = Vector3.zero;
            weddingRing.localRotation = Quaternion.identity;
        }

        public void SetMaterial(Material material)
        {
            var renderer = transform.Find("handModel/hand").GetComponent<MeshRenderer>();
            if (material)
            {
                renderer.material = material;
            } else
            {
                renderer.material.shader = Shader.Find("Standard");
            }
        }
    }
}
