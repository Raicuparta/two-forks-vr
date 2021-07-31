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
        private VRHand leftHand;
        private VRHand rightHand;
        private Animator animator;

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
            ToolPicker.Create(
                parent: instance.transform,
                leftHand: instance.leftHand.transform,
                rightHand: instance.rightHand.transform
            );
            VRHandLaser.Create(
                leftHand: instance.leftHand.transform,
                rightHand: instance.rightHand.transform
            );

            return instance;
        }

        public void SetUp(Transform playerTransform)
        {
            var henry = playerTransform?.Find("henry");
            animator = henry?.GetComponent<Animator>();
            var rootBone = henry?.Find("henryroot");
            rightHand.SetUp(rootBone, animator);
            leftHand.SetUp(rootBone, animator);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F11))
            {
                if (!animator)
                {
                    return;
                }
                MelonLogger.Msg("---- Start animation log ----");
                for (int layerIndex = 0; layerIndex < animator.layerCount; layerIndex++)
                {
                    if (animator.GetCurrentAnimatorClipInfoCount(layerIndex) == 0)
                    {
                        continue;
                    }
                    MelonLogger.Msg($"Layer Index: {layerIndex}");
                    MelonLogger.Msg($"Layer Name: {animator.GetLayerName(layerIndex)}");
                    var animations = animator.GetCurrentAnimatorClipInfo(layerIndex);
                    var animationNames = string.Join(", ", animations.Select(animation => animation.clip.name).ToArray());
                    MelonLogger.Msg($"Animations [{animationNames}]");
                }
                MelonLogger.Msg("---- End animation log ----");
            }
        }
    }
}
