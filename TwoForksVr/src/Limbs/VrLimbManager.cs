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
        private VrHand leftHand;
        private VrHand rightHand;
        private VrFoot rightFoot;
        private VrFoot leftFoot;
        private Laser laser;

        public static VrLimbManager Create(VrStage stage)
        {
            var instance = Instantiate(VrAssetLoader.Hands).AddComponent<VrLimbManager>();
            var instanceTransform = instance.transform;
            instanceTransform.SetParent(stage.transform, false);

            instance.rightHand = VrHand.Create(
                instanceTransform
            );
            instance.leftHand = VrHand.Create(
                instanceTransform,
                true
            );
            instance.rightFoot = VrFoot.Create();
            instance.leftFoot = VrFoot.Create(true);
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
            var henry = playerTransform != null ? playerTransform.Find("henry") : null;
            var rootBone = henry != null ? henry.Find("henryroot") : null;
            rightHand.SetUp(rootBone);
            leftHand.SetUp(rootBone);
            rightFoot.SetUp(rootBone);
            leftFoot.SetUp(rootBone);
            laser.SetUp(camera);
            GeneralDebugger.PlayerAnimator = henry != null ? henry.GetComponent<Animator>() : null;
        }
    }
}
