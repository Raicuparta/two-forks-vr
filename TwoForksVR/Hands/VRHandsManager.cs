using TwoForksVR.Tools;
using TwoForksVR.Assets;
using UnityEngine;
using TwoForksVR.Stage;
using TwoForksVR.Debug;

namespace TwoForksVR.Hands
{
    public class VRHandsManager: MonoBehaviour
    {
        private VRHand leftHand;
        private VRHand rightHand;

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
            var rootBone = henry?.Find("henryroot");
            rightHand.SetUp(rootBone);
            leftHand.SetUp(rootBone);
            GeneralDebugger.PlayerAnimator = henry?.GetComponent<Animator>();
        }
    }
}
