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
        private Transform playerTransform;

        public static VRHandsManager Create(Transform parent, Transform playerTransform)
        {
            var instance = Instantiate(VRAssetLoader.Hands).AddComponent<VRHandsManager>();
            instance.playerTransform = playerTransform;
            instance.transform.SetParent(parent, false);
            return instance;
        }

        private void Start()
        {
            SetUpHands();
        }

        private void SetUpHands()
        {
            var rootBone = playerTransform?.Find("henry/henryroot");
            var rightHand = VRHand.Create(
                parent: transform,
                rootBone: rootBone
            ).transform;
            var leftHand = VRHand.Create(
                parent: transform,
                rootBone: rootBone,
                isLeft: true
            ).transform;
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
    }
}
