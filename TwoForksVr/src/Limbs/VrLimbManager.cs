using System;
using TwoForksVr.LaserPointer;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using TwoForksVr.Tools;
using TwoForksVr.UI.Patches;
using UnityEngine;

namespace TwoForksVr.Limbs
{
    public class VrLimbManager : MonoBehaviour
    {
        public VrLaser Laser;
        private Transform henryTransform;
        private ToolPicker toolPicker;
        public VrHand NonDominantHand { get; private set; }
        public VrHand DominantHand { get; private set; }
        public bool IsToolPickerOpen => toolPicker && toolPicker.IsOpen;

        public static VrLimbManager Create(VrStage stage)
        {
            var instance = new GameObject("VrLimbManager").AddComponent<VrLimbManager>();
            var instanceTransform = instance.transform;
            instanceTransform.SetParent(stage.transform, false);

            instance.DominantHand = VrHand.Create(instanceTransform);
            instance.NonDominantHand = VrHand.Create(instanceTransform, true);
            instance.toolPicker = ToolPicker.Create(
                instanceTransform,
                instance.NonDominantHand.transform,
                instance.DominantHand.transform
            );
            instance.Laser = VrLaser.Create(instance.DominantHand.transform);

            InventoryPatches.DominantHand = instance.DominantHand.transform;

            return instance;
        }

        public void SetUp(vgPlayerController playerController, Camera camera)
        {
            var playerTransform = playerController ? playerController.transform : null;
            var skeletonRoot = GetSkeletonRoot(playerTransform);
            var armsMaterial = GetArmsMaterial(playerTransform);
            DominantHand.SetUp(skeletonRoot, armsMaterial, playerController);
            NonDominantHand.SetUp(skeletonRoot, armsMaterial, playerController);
            Laser.SetUp(camera);
            SetUpHandedness();
        }

        private void Awake()
        {
            VrSettings.LeftHandedMode.SettingChanged += HandleLeftHandedModeSettingChanged;
        }

        private void HandleLeftHandedModeSettingChanged(object sender, EventArgs e)
        {
            SetUpHandedness();
        }

        private static Material GetArmsMaterial(Transform playerTransform)
        {
            return !playerTransform
                ? null
                : playerTransform.Find("henry/body")?.GetComponent<SkinnedMeshRenderer>().materials[2];
        }

        private void SetUpHandedness()
        {
            if (!henryTransform) return;

            henryTransform.localScale = new Vector3(VrSettings.LeftHandedMode.Value ? -1 : 1, 1, 1);
        }

        private Transform GetSkeletonRoot(Transform playerTransform)
        {
            if (playerTransform == null) return null;

            henryTransform = playerTransform.Find("henry");

            return henryTransform.Find("henryroot");
        }
    }
}