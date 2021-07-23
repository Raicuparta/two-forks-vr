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
using TwoForksVR.Stage;

namespace TwoForksVR.Hands
{
    public class VRHandsManager: MonoBehaviour
    {
        public static VRHandsManager Create(VRStage stage)
        {
            var instance = Instantiate(VRAssetLoader.Hands).AddComponent<VRHandsManager>();
            instance.transform.SetParent(stage.transform, false);
            return instance;
        }

        public void SetUp(Transform playerTransform)
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
