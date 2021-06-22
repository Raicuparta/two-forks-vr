﻿using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.VR;
using Valve.VR;
// using Valve.VR;

namespace Raicuparta.TwoForksVR
{
    public class VRHand: MonoBehaviour
    {
        public bool IsLeft = false;

        private VRNode vrNode;

        private void Start()
        {
            gameObject.SetActive(false);
            name = $"{(IsLeft ? "Left" : "Right")} Hand";
            transform.SetParent(Camera.main.transform.parent, false); // TODO make sure camera is initialized?
            vrNode = IsLeft ? VRNode.LeftHand : VRNode.RightHand;
            var pose = gameObject.AddComponent<SteamVR_Behaviour_Pose>();

            if (IsLeft)
            {
                var handModel = transform.Find("handModel");
                handModel.localScale = new Vector3(-handModel.localScale.x, handModel.localScale.y, handModel.localScale.z);
                SetUpWeddingRing();
                SetUpMap();
                pose.inputSource = SteamVR_Input_Sources.LeftHand;
                pose.poseAction = SteamVR_Actions.default_PoseLeftHand;
            } else
            {
                var handLaser = new GameObject().AddComponent<VRHandLaser>().transform;
                handLaser.SetParent(transform, false);
                pose.inputSource = SteamVR_Input_Sources.RightHand;
                pose.poseAction = SteamVR_Actions.default_PoseRightHand;

                var toolPicker = gameObject.AddComponent<VRToolPicker>();
                toolPicker.ParentWhileActive = Camera.main.transform.parent;
                toolPicker.Hand = transform;
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