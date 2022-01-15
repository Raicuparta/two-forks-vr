using TwoForksVr.Assets;
using TwoForksVr.Debugging;
using TwoForksVr.LaserPointer;
using TwoForksVr.Stage;
using TwoForksVr.Tools;
using TwoForksVr.UI.Patches;
using UnityEngine;

namespace TwoForksVr.Limbs
{
    public class VrLimbManager : MonoBehaviour
    {
        private Laser laser;
        private VrHand leftHand;
        private VrHand rightHand;
        private Transform vrBody;

        public static VrLimbManager Create(VrStage stage)
        {
            var instance = Instantiate(VrAssetLoader.HandsPrefab, stage.transform, false).AddComponent<VrLimbManager>();
            var instanceTransform = instance.transform;

            instance.rightHand = VrHand.Create(
                instanceTransform
            );
            instance.leftHand = VrHand.Create(
                instanceTransform,
                true
            );
            ToolPicker.Create(
                instanceTransform,
                instance.leftHand.transform,
                instance.rightHand.transform
            );
            instance.laser = Laser.Create(
                instance.leftHand.transform,
                instance.rightHand.transform
            );

            instance.vrBody = Instantiate(VrAssetLoader.PlayerPrefab, instanceTransform, false).transform;

            InventoryPatches.RightHand = instance.rightHand.transform;

            return instance;
        }

        public void SetUp(Transform playerTransform, Camera camera)
        {
            var cloneHenry = vrBody.Find("PlayerModel/henry");
            var cloneRootBone = cloneHenry.Find("henryroot");
            var animatedHenry = playerTransform ? playerTransform.Find("henry") : null;
            var animatedRootBone = animatedHenry ? animatedHenry.Find("henryroot") : null;
            rightHand.SetUp(animatedRootBone, cloneRootBone);
            leftHand.SetUp(animatedRootBone, cloneRootBone);
            laser.SetUp(camera);
            GeneralDebugger.PlayerAnimator = animatedHenry != null ? animatedHenry.GetComponent<Animator>() : null;


            VrFoot.Create(animatedRootBone);
            VrFoot.Create(animatedRootBone, true);
        }
    }
}