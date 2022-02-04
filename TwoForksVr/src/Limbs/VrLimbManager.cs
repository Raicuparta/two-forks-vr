using TwoForksVr.LaserPointer;
using TwoForksVr.Stage;
using TwoForksVr.Tools;
using TwoForksVr.UI.Patches;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.Limbs
{
    public class VrLimbManager : MonoBehaviour
    {
        private Laser laser;
        private ToolPicker toolPicker;
        public VrHand LeftHand { get; private set; }
        public VrHand RightHand { get; private set; }
        public bool IsToolPickerOpen => toolPicker && toolPicker.IsOpen;

        public static VrLimbManager Create(VrStage stage)
        {
            var instance = new GameObject("VrLimbManager").AddComponent<VrLimbManager>();
            var instanceTransform = instance.transform;
            instanceTransform.SetParent(stage.transform, false);

            instance.RightHand = VrHand.Create(instanceTransform);
            instance.LeftHand = VrHand.Create(instanceTransform, true);
            instance.toolPicker = ToolPicker.Create(
                instanceTransform,
                instance.LeftHand.transform,
                instance.RightHand.transform
            );
            instance.laser = Laser.Create(
                instance.LeftHand.transform,
                instance.RightHand.transform
            );

            InventoryPatches.RightHand = instance.RightHand.transform;

            return instance;
        }

        public void SetUp(Transform playerTransform, Camera camera)
        {
            var skeletonRoot = GetSkeletonRoot(playerTransform);
            var armsMaterial = GetArmsMaterial(playerTransform);
            RightHand.SetUp(skeletonRoot, armsMaterial);
            LeftHand.SetUp(skeletonRoot, armsMaterial);
            laser.SetUp(camera);

            VrFoot.Create(skeletonRoot);
            VrFoot.Create(skeletonRoot, true);
        }

        public void HighlightButton(params ISteamVR_Action_In_Source[] actions)
        {
            if (actions.Length == 0)
            {
                LeftHand.ButtonHighlight.HideAllButtonHints();
                RightHand.ButtonHighlight.HideAllButtonHints();
            }
            else
            {
                LeftHand.ButtonHighlight.ShowButtonHint(actions);
                RightHand.ButtonHighlight.ShowButtonHint(actions);
            }
        }

        private static Material GetArmsMaterial(Transform playerTransform)
        {
            return !playerTransform
                ? null
                : playerTransform.Find("henry/body")?.GetComponent<SkinnedMeshRenderer>().materials[2];
        }

        private static Transform GetSkeletonRoot(Transform playerTransform)
        {
            return playerTransform ? playerTransform.Find("henry/henryroot") : null;
        }
    }
}