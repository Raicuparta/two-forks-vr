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

        public static VrLimbManager Create(VrStage stage)
        {
            var instance = Instantiate(VrAssetLoader.HandsPrefab, stage.transform, false).AddComponent<VrLimbManager>();
            var instanceTransform = instance.transform;

            var vrBody = Instantiate(VrAssetLoader.PlayerPrefab, instanceTransform, false).transform;
            var cloneHenry = vrBody.Find("PlayerModel/henry");
            var cloneRootBone = cloneHenry.Find("henryroot");
            
            instance.rightHand = VrHand.Create(
                instanceTransform,
                cloneRootBone
            );
            instance.leftHand = VrHand.Create(
                instanceTransform,
                cloneRootBone,
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

            InventoryPatches.RightHand = instance.rightHand.transform;

            return instance;
        }

        public void SetUp(Transform playerTransform, Camera camera)
        {
            var animatedHenry = playerTransform ? playerTransform.Find("henry") : null;
            // if (animatedHenry)
            // {
            //     var clonedRenderer = cloneHenry.Find("body").GetComponent<SkinnedMeshRenderer>().materials[1];
            //     var animatedArmsMaterial = animatedHenry.Find("body").GetComponent<SkinnedMeshRenderer>().materials[2];
            //     clonedRenderer.shader = animatedArmsMaterial.shader;
            // }
            var animatedRootBone = animatedHenry ? animatedHenry.Find("henryroot") : null;
            rightHand.SetUp(animatedRootBone);
            leftHand.SetUp(animatedRootBone);
            laser.SetUp(camera);
            GeneralDebugger.PlayerAnimator = animatedHenry != null ? animatedHenry.GetComponent<Animator>() : null;


            VrFoot.Create(animatedRootBone);
            VrFoot.Create(animatedRootBone, true);
        }
    }
}