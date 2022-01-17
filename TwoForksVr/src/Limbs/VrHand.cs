using System;
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

        private void Start()
        {
            // Reset the tracking. For some reason if I don't do this the hands will be frozen.
            gameObject.SetActive(true);
        }
        // private Transform rootBone;

        public static VrHand Create(Transform parent, Transform cloneRootBone, bool isLeft = false)
        {
            var handName = isLeft ? "Left" : "Right";
            // var transform = parent.Find($"{handName}Hand");
            var transform = Instantiate(isLeft ? VrAssetLoader.LeftHandPrefab : VrAssetLoader.RightHandPrefab, parent, false).transform;
            transform.name = $"{handName}Hand";
            var instance = transform.gameObject.AddComponent<VrHand>();
            instance.handName = handName;
            instance.isLeft = isLeft;
            instance.SetUpPose();
            // instance.cloneRootBone = cloneRootBone;
            return instance;
        }

        public void SetUp(Transform playerRootBone)
        {
            gameObject.SetActive(false);
            EnableAnimatedHand(playerRootBone);
            gameObject.SetActive(true);
        }

        private void SetUpPose()
        {
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
        }

        private void FollowAllChildrenRecursive(Transform clone, Transform target)
        {
            foreach (Transform cloneChild in clone)
            {
                var targetChild = target.Find(cloneChild.name);
                if (targetChild)
                {
                    cloneChild.gameObject.AddComponent<FollowLocalTransform>().Target = targetChild;
                    FollowAllChildrenRecursive(cloneChild, target.Find(cloneChild.name));
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
            if (!animatedArmBone)
            {
                Logs.LogWarning("found no animated arm bone");
            }
            if (animatedArmBone)
            {
                var clonedArmBone = transform.Find($"henry/henryroot/henryPelvis/henryArm{handName}Hand");
                if (!clonedArmBone)
                {
                    Logs.LogError("found no cloned arm bone");
                }
                FollowAllChildrenRecursive(clonedArmBone, animatedArmBone);
            }
            
            // SetUpHandLid(armBone);
            if (animatedArmBone)
            {
                SetUpHandBone(animatedArmBone);
            }
        }

        private Transform SetUpArmBone(Transform playerRootBone)
        {
            if (!playerRootBone) return null;

            var armBone =
                playerRootBone.Find(
                    $"henryPelvis/henrySpineA/henrySpineB/henrySpineC/henrySpineD/henrySpider{handName}1/henrySpider{handName}2/henrySpider{handName}IK/henryArm{handName}Collarbone/henryArm{handName}1/henryArm{handName}2");
            var updateFollow = armBone.GetComponent<LateUpdateFollow>() ??
                               armBone.gameObject.AddComponent<LateUpdateFollow>();
            updateFollow.Target = transform.Find("ArmTarget");
            return armBone.Find($"henryArm{handName}Hand");
        }

        private void SetUpHandLid(Transform armBone)
        {
            var handLid = Instantiate(VrAssetLoader.HandLid).transform;
            LayerHelper.SetLayer(handLid.Find("HandLidModel"), GameLayer.PlayerBody);
            handLid.SetParent(armBone, false);
            if (isLeft) handLid.localScale = new Vector3(1, 1, -1);
        }

        private void SetUpHandBone(Transform armBoneChild)
        {
            var armBone = armBoneChild.parent; // TODO cleanup;
            var wristTargetName = $"{handName}WristTarget";
            var wristTarget = armBone.Find(wristTargetName) ?? new GameObject(wristTargetName).transform;
            wristTarget.SetParent(armBone);
            var stabilizerAngleMultiplier = isLeft ? -1 : 1;
            wristTarget.localPosition = new Vector3(-0.2497151f, 0f, 0f);
            wristTarget.localEulerAngles = new Vector3(3.949f * stabilizerAngleMultiplier,
                17.709f * stabilizerAngleMultiplier, 12.374f);
            var handBone = armBone.transform.Find($"henryArm{handName}Hand");
            var handBoneFollow = handBone.GetComponent<LateUpdateFollow>() ??
                                 handBone.gameObject.AddComponent<LateUpdateFollow>();
            handBoneFollow.Target = wristTarget;
        }
    }
}