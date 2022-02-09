using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.VrInput.ActionInputs;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.Limbs
{
    public class VrHand : MonoBehaviour
    {
        private string handName;
        private FakeParenting handRootFakeParenting;
        private bool isLeft;
        private vgPlayerNavigationController navigationController;
        public VrButtonHighlight ButtonHighlight { get; private set; }

        public static VrHand Create(Transform parent, bool isLeft = false)
        {
            var handName = isLeft ? "Left" : "Right";
            var transform = Instantiate(isLeft ? VrAssetLoader.LeftHandPrefab : VrAssetLoader.RightHandPrefab, parent,
                false).transform;
            LayerHelper.SetLayerRecursive(transform.gameObject, GameLayer.UI);
            transform.name = $"{handName}Hand";
            var instance = transform.gameObject.AddComponent<VrHand>();
            instance.handName = handName;
            instance.isLeft = isLeft;
            instance.ButtonHighlight = transform.GetComponentInChildren<VrButtonHighlight>();
            instance.SetUpPose();

            return instance;
        }

        public void SetUp(Transform playerRootBone, Material armsMaterial, vgPlayerController playerController)
        {
            // Need to deactive and reactivate the object to make SteamVR_Behaviour_Pose work properly.
            gameObject.SetActive(false);
            if (armsMaterial)
            {
                var material = GetComponentInChildren<SkinnedMeshRenderer>().material;
                material.shader = armsMaterial.shader;
                material.CopyPropertiesFromMaterial(armsMaterial);
            }

            if (playerController) navigationController = playerController.navController;

            EnableAnimatedHand(playerRootBone);
            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!navigationController || !handRootFakeParenting) return;

            handRootFakeParenting.enabled = navigationController.enabled;
        }

        private void SetUpPose()
        {
            var pose = gameObject.GetComponent<SteamVR_Behaviour_Pose>();
            if (isLeft)
            {
                pose.inputSource = SteamVR_Input_Sources.LeftHand;
                pose.poseAction = ActionInputDefinitions.ActionSet.PoseLeftHand;
            }
            else
            {
                pose.inputSource = SteamVR_Input_Sources.RightHand;
                pose.poseAction = ActionInputDefinitions.ActionSet.PoseRightHand;
            }
        }

        private void FollowAllChildrenRecursive(Transform clone, Transform target)
        {
            foreach (Transform cloneChild in clone)
            {
                var targetChild = target.Find(cloneChild.name);
                if (!targetChild) continue;

                // Wedding ring and hand root are special cases, the originals need to follow the copies.
                var isCloneHandRoot = cloneChild.name.Equals($"henryArm{handName}Hand");
                if (isCloneHandRoot)
                    handRootFakeParenting = FakeParenting.Create(targetChild, cloneChild,
                        FakeParenting.UpdateType.LateUpdate | FakeParenting.UpdateType.VeryLateUpdate);

                var isCloneWeddingRing = cloneChild.name.Equals("HenryWeddingRing 1");
                if (isCloneWeddingRing) FakeParenting.Create(targetChild, cloneChild);

                if (isCloneWeddingRing) continue;

                FollowAllChildrenRecursive(cloneChild, target.Find(cloneChild.name));

                if (isCloneHandRoot) continue;

                // Clone hand bones will follow the original bones, to mimick the same animations.
                cloneChild.gameObject.AddComponent<CopyLocalTransformValues>().Target = targetChild;
            }
        }

        private void EnableAnimatedHand(Transform animatedRootBone)
        {
            if (!animatedRootBone) return;
            var animatedArmBone =
                animatedRootBone.Find(
                    $"henryPelvis/henrySpineA/henrySpineB/henrySpineC/henrySpineD/henrySpider{handName}1/henrySpider{handName}2/henrySpider{handName}IK/henryArm{handName}Collarbone/henryArm{handName}1/henryArm{handName}2");

            var clonedArmBone = transform.Find("henry/henryroot/henryPelvis");
            if (!clonedArmBone) Logs.LogError("found no cloned arm bone");
            FollowAllChildrenRecursive(clonedArmBone, animatedArmBone);
        }
    }
}