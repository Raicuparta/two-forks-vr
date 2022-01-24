using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.TeleportLocomotion;
using TwoForksVr.UI;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.Limbs
{
    public class VrHand : MonoBehaviour
    {
        public VrButtonHighlight ButtonHighlight { get; private set; }
        private string handName;
        private bool isLeft;

        public static VrHand Create(Transform parent, bool isLeft = false)
        {
            var handName = isLeft ? "Left" : "Right";
            var transform = Instantiate(isLeft ? VrAssetLoader.LeftHandPrefab : VrAssetLoader.RightHandPrefab, parent, false).transform;
            LayerHelper.SetLayerRecursive(transform.gameObject, GameLayer.UI);
            transform.name = $"{handName}Hand";
            var instance = transform.gameObject.AddComponent<VrHand>();
            instance.handName = handName;
            instance.isLeft = isLeft;
            instance.ButtonHighlight = transform.GetComponentInChildren<VrButtonHighlight>();
            instance.SetUpPose();

            return instance;
        }

        public void SetUp(Transform playerRootBone, Material armsMaterial)
        {
            // Need to deactive and reactivate the object to make SteamVR_Behaviour_Pose work properly.
            gameObject.SetActive(false);
            if (armsMaterial)
            {
                var material = GetComponentInChildren<SkinnedMeshRenderer>().material;
                material.shader = armsMaterial.shader;
                material.CopyPropertiesFromMaterial(armsMaterial);
            }
            EnableAnimatedHand(playerRootBone);
            gameObject.SetActive(true);
        }

        private void SetUpPose()
        {
            var pose = gameObject.GetComponent<SteamVR_Behaviour_Pose>();
            if (isLeft)
            {
                pose.inputSource = SteamVR_Input_Sources.LeftHand;
                pose.poseAction = SteamVR_Actions.default_PoseLeftHand;
            }
            else
            {
                pose.inputSource = SteamVR_Input_Sources.RightHand;
                pose.poseAction = SteamVR_Actions.default_PoseRightHand;
            }
        }

        private void FollowAllChildrenRecursive(Transform clone, Transform target)
        {
            foreach (Transform cloneChild in clone)
            {
                var targetChild = target.Find(cloneChild.name);
                if (targetChild)
                {
                    // Wedding ring and attachment objects are special cases, the originals need to follow the copies.
                    // The "hand attachment" transform is what's used for holding objects in the palyer's hand.
                    var isCloneAttachment = cloneChild.name.Equals($"henryHand{handName}Attachment");
                    var isCloneWeddingRing = cloneChild.name.Equals("HenryWeddingRing 1");
                    if (isCloneWeddingRing || isCloneAttachment)
                    {
                        targetChild.gameObject.AddComponent<FakeParenting>().Target = cloneChild;
                    }
                    
                    if (!isCloneWeddingRing)
                    {
                        cloneChild.gameObject.AddComponent<FollowLocalTransform>().Target = targetChild;
                        FollowAllChildrenRecursive(cloneChild, target.Find(cloneChild.name));
                    }
                }
                else
                {
                    Logs.LogInfo($"Found no child in ${target.name} with name ${cloneChild.name}");
                }
            }
        }

        private void EnableAnimatedHand(Transform animatedRootBone)
        {
            var animatedArmBone = SetUpArmBone(animatedRootBone);
            if (!animatedArmBone) return;

            var clonedArmBone = transform.Find($"henry/henryroot/henryPelvis/henryArm{handName}Hand");
            if (!clonedArmBone)
            {
                Logs.LogError("found no cloned arm bone");
            }
            FollowAllChildrenRecursive(clonedArmBone, animatedArmBone);
        }

        private Transform SetUpArmBone(Transform playerRootBone)
        {
            if (!playerRootBone) return null;

            var armBone =
                playerRootBone.Find(
                    $"henryPelvis/henrySpineA/henrySpineB/henrySpineC/henrySpineD/henrySpider{handName}1/henrySpider{handName}2/henrySpider{handName}IK/henryArm{handName}Collarbone/henryArm{handName}1/henryArm{handName}2/henryArm{handName}Hand");

            // The original hands are hidden but I still make them follow the fake hands,
            // just for any behaviours that rely on the real hand transform.
            // I didn't bother making it follow the position and rotation precisely,
            // since I only cared about fixing the map cloth movement.
            armBone.gameObject.AddComponent<FakeParenting>().Target = transform;

            return armBone;
        }
    }
}