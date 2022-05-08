using System;
using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.Settings;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.Limbs;

public class VrHand : MonoBehaviour
{
    private FakeParenting handRootFakeParenting;
    private bool isDominant;
    private Transform rootBone;
    private Renderer[] renderers;

    public static VrHand Create(Transform parent, bool isNonDominant = false)
    {
        var transform = Instantiate(isNonDominant ? VrAssetLoader.LeftHandPrefab : VrAssetLoader.RightHandPrefab,
            parent,
            false).transform;
        LayerHelper.SetLayerRecursive(transform.gameObject, GameLayer.UI);
        transform.name = $"{(isNonDominant ? "Dominand" : "NonDominant")}VrHand";
        var instance = transform.gameObject.AddComponent<VrHand>();
        instance.isDominant = isNonDominant;

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

        rootBone = playerRootBone;

        SetUpSettings();

        gameObject.SetActive(true);
    }

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnEnable()
    {
        VrSettings.Config.SettingChanged += HandleSettingChanged;
    }

    private void OnDisable()
    {
        VrSettings.Config.SettingChanged -= HandleSettingChanged;
    }

    private void HandleSettingChanged(object sender, EventArgs e)
    {
        SetUpSettings();
    }

    private void SetUpSettings()
    {
        SetUpHandedness();
        SetUpHandVisibility();
    }

    private void SetUpHandVisibility()
    {
        if (renderers == null) return;
        foreach (var renderer in renderers)
        {
            renderer.enabled = VrSettings.ShowVrHands.Value;
        }
    }

    private void SetUpHandedness()
    {
        var isLeft = VrSettings.LeftHandedMode.Value ? !isDominant : isDominant;

        // In Firewatch, Henry is right-handed. He holds the radio with his left hand,
        // but he picks up and throws objects with his right hand. To allow for left handedness in VR, we need to
        // swap the original hands. We can achieve this by targetting different hand and arm bones.
        // So, when playing in left handed mode, the right hand will have armBoneName = "Left",
        // because the player is controlling Henry's left hand with their right VR controller.
        var armBoneName = isDominant ? "Left" : "Right";
        transform.localScale = new Vector3(VrSettings.LeftHandedMode.Value ? -1 : 1, 1, 1);
        EnableAnimatedHand(armBoneName);
        SetUpPose(isLeft);
    }

    private void SetUpPose(bool isLeft)
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

    private void FollowAllChildrenRecursive(Transform clone, Transform target, string handName)
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

            FollowAllChildrenRecursive(cloneChild, target.Find(cloneChild.name), handName);

            if (isCloneHandRoot) continue;

            // Clone hand bones will follow the original bones, to mimick the same animations.
            CopyLocalTransformValues.Create(cloneChild.gameObject, targetChild);
        }
    }

    private void EnableAnimatedHand(string handName)
    {
        if (!rootBone) return;

        var armBone = rootBone.Find(
            $"henryPelvis/henrySpineA/henrySpineB/henrySpineC/henrySpineD/henrySpider{handName}1/henrySpider{handName}2/henrySpider{handName}IK/henryArm{handName}Collarbone/henryArm{handName}1/henryArm{handName}2");

        var clonedArmBone = transform.Find("henry/henryroot/henryPelvis");
        if (!clonedArmBone) Logs.WriteError("found no cloned arm bone");
        FollowAllChildrenRecursive(clonedArmBone, armBone, handName);
    }

    public void StopTrackingOriginalHands()
    {
        if (!handRootFakeParenting) return;

        handRootFakeParenting.enabled = false;
    }

    public void StartTrackingOriginalHands()
    {
        if (!handRootFakeParenting) return;

        handRootFakeParenting.enabled = true;
    }
}