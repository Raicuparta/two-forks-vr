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
        private Transform leftHand;
        private Transform rightHand;
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
            SetUpHands();
            ToolPicker.Create(
                parent: transform,
                leftHand: leftHand,
                rightHand: rightHand
            );
            VRHandLaser.Create(
                leftHand: leftHand,
                rightHand: rightHand
            );
        }

        private void SetUpHands()
        {
            var rootBone = playerBody?.parent.Find("henryroot");

            rightHand = VRHand.Create(
                parent: transform,
                rootBone: rootBone
            ).transform;
            leftHand = VRHand.Create(
                parent: transform,
                rootBone: rootBone,
                isLeft: true
            ).transform;
        }
    }
}
