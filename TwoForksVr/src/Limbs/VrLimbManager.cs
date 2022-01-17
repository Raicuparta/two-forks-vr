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
            var instance = new GameObject().AddComponent<VrLimbManager>();
            var instanceTransform = instance.transform;
            instanceTransform.SetParent(stage.transform, false);
            
            instance.rightHand = VrHand.Create(instanceTransform);
            instance.leftHand = VrHand.Create(instanceTransform, true);
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
            var animatedArmsMaterial = animatedHenry ? animatedHenry.Find("body").GetComponent<SkinnedMeshRenderer>().materials[2] : null;
            var animatedRootBone = animatedHenry ? animatedHenry.Find("henryroot") : null;
            rightHand.SetUp(animatedRootBone, animatedArmsMaterial);
            leftHand.SetUp(animatedRootBone, animatedArmsMaterial);
            laser.SetUp(camera);
            GeneralDebugger.PlayerAnimator = animatedHenry != null ? animatedHenry.GetComponent<Animator>() : null;


            VrFoot.Create(animatedRootBone);
            VrFoot.Create(animatedRootBone, true);
        }
    }
}