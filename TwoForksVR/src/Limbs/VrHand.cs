using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.Limbs
{
    public class VrHand : MonoBehaviour
    {
        private GameObject fallbackHandModel;
        private string handName;
        private bool isLeft;
        // private Transform rootBone;

        public static VrHand Create(Transform parent, bool isLeft = false)
        {
            var handName = isLeft ? "Left" : "Right";
            var transform = parent.Find($"{handName}Hand");
            var instance = transform.gameObject.AddComponent<VrHand>();
            instance.handName = handName;
            instance.isLeft = isLeft;
            instance.fallbackHandModel = transform.Find("HandModel")?.gameObject;
            instance.SetUpPose();
            return instance;
        }

        public void SetUp(Transform playerRootBone)
        {
            gameObject.SetActive(false);
            if (playerRootBone)
            {
                SetFallbackHandActive(false);
                EnableAnimatedHand(playerRootBone);
            }
            else
            {
                SetFallbackHandActive(true);
            }

            gameObject.SetActive(true);
        }
        
        private void Start()
        {
            // Reset the tracking. For some reason if I don't do this the hands will be frozen.
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

        private void SetFallbackHandActive(bool active)
        {
            if (!fallbackHandModel) return;
            fallbackHandModel.SetActive(active);
        }

        private void EnableAnimatedHand(Transform playerRootBone)
        {
            if (!playerRootBone) return;

            var armBone = SetUpArmBone(playerRootBone);
            SetUpHandLid(armBone);
            SetUpHandBone(armBone);
        }

        private Transform SetUpArmBone(Transform playerRootBone)
        {
            var armBone =
                playerRootBone.Find(
                    $"henryPelvis/henrySpineA/henrySpineB/henrySpineC/henrySpineD/henrySpider{handName}1/henrySpider{handName}2/henrySpider{handName}IK/henryArm{handName}Collarbone/henryArm{handName}1/henryArm{handName}2");
            var updateFollow = armBone.GetComponent<LateUpdateFollow>() ??
                               armBone.gameObject.AddComponent<LateUpdateFollow>();
            updateFollow.Target = transform.Find("ArmTarget");
            return armBone;
        }
        
        private void SetUpHandLid(Transform armBone)
        {
            var handLid = Instantiate(VrAssetLoader.HandLid).transform;
            LayerHelper.SetLayer(handLid.Find("HandLidModel"), GameLayer.PlayerBody);
            handLid.SetParent(armBone, false);
            if (isLeft) handLid.localScale = new Vector3(1, 1, -1);
        }

        private void SetUpHandBone(Transform armBone)
        {
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