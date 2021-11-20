using TwoForksVr.Assets;
using TwoForksVr.Debug;
using TwoForksVr.Stage;
using TwoForksVr.Tools;
using TwoForksVr.UI.Patches;
using UnityEngine;

namespace TwoForksVr.Hands
{
    public class VRHandsManager : MonoBehaviour
    {
        private VRHand leftHand;
        private VRHand rightHand;

        public static VRHandsManager Create(VRStage stage)
        {
            var instance = Instantiate(VRAssetLoader.Hands).AddComponent<VRHandsManager>();
            var instanceTransform = instance.transform;
            instanceTransform.SetParent(stage.transform, false);

            instance.rightHand = VRHand.Create(
                instanceTransform
            );
            instance.leftHand = VRHand.Create(
                instanceTransform,
                true
            );
            ToolPicker.Create(
                instanceTransform,
                instance.leftHand.transform,
                instance.rightHand.transform
            );
            VRHandLaser.Create(
                instance.leftHand.transform,
                instance.rightHand.transform
            );

            InventoryPatches.RightHand = instance.rightHand.transform;

            return instance;
        }

        public void SetUp(Transform playerTransform)
        {
            var henry = playerTransform != null ? playerTransform.Find("henry") : null;
            var rootBone = henry != null ? henry.Find("henryroot") : null;
            rightHand.SetUp(rootBone);
            leftHand.SetUp(rootBone);
            GeneralDebugger.PlayerAnimator = henry != null ? henry.GetComponent<Animator>() : null;
        }
    }
}
