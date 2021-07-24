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
        VRHand leftHand;
        VRHand rightHand;
        ToolPicker toolPicker;
        VRHandLaser handLaser;

        public static VRHandsManager Create(VRStage stage)
        {
            var instance = Instantiate(VRAssetLoader.Hands).AddComponent<VRHandsManager>();
            instance.transform.SetParent(stage.transform, false);

            instance.rightHand = VRHand.Create(
                parent: instance.transform
            );
            instance.leftHand = VRHand.Create(
                parent: instance.transform,
                isLeft: true
            );
            instance.toolPicker = ToolPicker.Create(
                parent: instance.transform,
                leftHand: instance.leftHand.transform,
                rightHand: instance.rightHand.transform
            );
            instance.handLaser = VRHandLaser.Create(
                leftHand: instance.leftHand.transform,
                rightHand: instance.rightHand.transform
            );

            return instance;
        }

        public void SetUp(Transform playerTransform)
        {
            var rootBone = playerTransform?.Find("henry/henryroot");
            rightHand.SetUp(rootBone);
            leftHand.SetUp(rootBone);
        }
    }
}
