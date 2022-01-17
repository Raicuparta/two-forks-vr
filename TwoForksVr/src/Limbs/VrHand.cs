using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.Limbs
{
    public class VrHand : MonoBehaviour
    {
        private string handName;
        private bool isLeft;

        public static VrHand Create(Transform parent, bool isLeft = false)
        {
            var handName = isLeft ? "Left" : "Right";
            var transform = Instantiate(isLeft ? VrAssetLoader.LeftHandPrefab : VrAssetLoader.RightHandPrefab, parent, false).transform;
            transform.name = $"{handName}Hand";
            var instance = transform.gameObject.AddComponent<VrHand>();
            instance.handName = handName;
            instance.isLeft = isLeft;
            instance.SetUpPose();
            return instance;
        }

        public void SetUp(Transform playerRootBone, Material armsMaterial)
        {
            if (armsMaterial)
            {
                var material = GetComponentInChildren<SkinnedMeshRenderer>().material;
                material.shader = armsMaterial.shader;
                material.CopyPropertiesFromMaterial(armsMaterial);
            }
            EnableAnimatedHand(playerRootBone);
        }

        private void SetUpPose()
        {
            // Need to deactive and reactivate the object to make SteamVR_Behaviour_Pose work properly.
            gameObject.SetActive(false);
            var pose = gameObject.AddComponent<SteamVR_Behaviour_Pose>();
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
            gameObject.SetActive(true);
        }

        private void FollowAllChildrenRecursive(Transform clone, Transform target)
        {
            foreach (Transform cloneChild in clone)
            {
                var targetChild = target.Find(cloneChild.name);
                if (targetChild)
                {
                    var isCloneWeddingRing = cloneChild.name.Equals("HenryWeddingRing 1");
                    var isCloneAttachment = cloneChild.name.Equals($"henryHand{handName}Attachment");
                    if (isCloneWeddingRing || isCloneAttachment)
                    {
                        targetChild.gameObject.AddComponent<LateUpdateFollow>().Target = cloneChild;
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
            var animatedArmBone = GetArmBone(animatedRootBone);
            if (animatedArmBone)
            {
                var clonedArmBone = transform.Find($"henry/henryroot/henryPelvis/henryArm{handName}Hand");
                if (!clonedArmBone)
                {
                    Logs.LogError("found no cloned arm bone");
                }
                FollowAllChildrenRecursive(clonedArmBone, animatedArmBone);
            }
        }

        private Transform GetArmBone(Transform playerRootBone)
        {
            if (!playerRootBone) return null;

            var armBone =
                playerRootBone.Find(
                    $"henryPelvis/henrySpineA/henrySpineB/henrySpineC/henrySpineD/henrySpider{handName}1/henrySpider{handName}2/henrySpider{handName}IK/henryArm{handName}Collarbone/henryArm{handName}1/henryArm{handName}2");
            return armBone.Find($"henryArm{handName}Hand");
        }
    }
}