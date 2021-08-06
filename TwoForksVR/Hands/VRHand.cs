using UnityEngine;
using Valve.VR;
using TwoForksVR.Assets;
using TwoForksVR.Helpers;

namespace TwoForksVR.Hands
{
    public class VRHand: MonoBehaviour
    {
        private bool isLeft;
        private Transform rootBone;
        private string handName;
        private GameObject fallbackHandModel;

        public static VRHand Create(Transform parent,  bool isLeft = false)
        {
            var handName = isLeft ? "Left" : "Right";
            var transform = parent.Find($"{handName}Hand");
            var instance = transform.gameObject.AddComponent<VRHand>();
            instance.handName = handName;
            instance.isLeft = isLeft;
            instance.fallbackHandModel = transform.Find("HandModel")?.gameObject;
            instance.SetUpPose();
            return instance;
        }

        public void SetUp(Transform rootBone)
        {
            gameObject.SetActive(false);
            this.rootBone = rootBone;
            if (rootBone)
            {
                SetFallbackHandActive(false);
                EnableAnimatedHand();
            }
            else
            {
                SetFallbackHandActive(true);
            }
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

        private void Start()
        {
            // Reset the tracking. For some reason if I don't do this the hands will be frozen.
            gameObject.SetActive(true);
        }

        private void SetFallbackHandActive(bool active)
        {
            if (!fallbackHandModel) return;
            fallbackHandModel.SetActive(active);
        }

        private void EnableAnimatedHand()
        {
            if (!rootBone)
            {
                return;
            }

            var armBone = SetUpArmBone();
            SetUpHandLid(armBone);
            SetUpHandBone(armBone);
        }

        private Transform SetUpArmBone()
        {
            var armBone = rootBone.Find($"henryPelvis/henrySpineA/henrySpineB/henrySpineC/henrySpineD/henrySpider{handName}1/henrySpider{handName}2/henrySpider{handName}IK/henryArm{handName}Collarbone/henryArm{handName}1/henryArm{handName}2");
            var updateFollow = armBone.GetComponent<LateUpdateFollow>() ?? armBone.gameObject.AddComponent<LateUpdateFollow>();
            updateFollow.Target = transform.Find("ArmTarget");
            return armBone;
        }

        private void SetUpHandLid(Transform armBone)
        {
            var handLid = Instantiate(VRAssetLoader.HandLid).transform;
            handLid.SetParent(armBone, false);
            if (isLeft)
            {
                handLid.localScale = new Vector3(1, 1, -1);
            }
        }

        private Transform SetUpHandBone(Transform armBone)
        {
            var wristTargetName = $"{handName}WristTarget";
            var wristTarget = armBone.Find(wristTargetName) ?? new GameObject(wristTargetName).transform;
            wristTarget.SetParent(armBone);
            var stabilizerAngleMultiplier = isLeft ? -1 : 1;
            wristTarget.localPosition = new Vector3(-0.2497151f, 0f, 0f);
            wristTarget.localEulerAngles = new Vector3(3.949f * stabilizerAngleMultiplier, 17.709f * stabilizerAngleMultiplier, 12.374f);
            var handBone = armBone.transform.Find($"henryArm{handName}Hand");
            var handBoneFollow = handBone.GetComponent<LateUpdateFollow>() ?? handBone.gameObject.AddComponent<LateUpdateFollow>();
            handBoneFollow.Target = wristTarget;
            return handBone;
        }
    }
}
