﻿using System;
using TwoForksVr.LaserPointer;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using TwoForksVr.Tools;
using UnityEngine;

namespace TwoForksVr.Limbs;

public class VrLimbManager : MonoBehaviour
{
    public VrLaser Laser;
    private Transform henryTransform;
    private vgPlayerNavigationController navigationController;
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
        instance.toolPicker = ToolPicker.Create(instance, instance.DominantHand);
        instance.Laser = VrLaser.Create(instance.DominantHand.transform);

        return instance;
    }

    public void SetUp(vgPlayerController playerController, Camera camera)
    {
        var playerTransform = playerController ? playerController.transform : null;
        navigationController = playerController ? playerController.navController : null;
        var skeletonRoot = GetSkeletonRoot(playerTransform);
        var armsMaterial = GetArmsMaterial(playerTransform);
        DominantHand.SetUp(skeletonRoot, armsMaterial);
        NonDominantHand.SetUp(skeletonRoot, armsMaterial);
        Laser.SetUp(camera);
        UpdateHandedness();
    }

    private void Update()
    {
        UpdateHandedness();
    }

    private void OnEnable()
    {
        VrSettings.Config.SettingChanged += HandleLeftHandedModeSettingChanged;
    }

    private void OnDisable()
    {
        VrSettings.Config.SettingChanged -= HandleLeftHandedModeSettingChanged;
    }

    private VrHand GetRightHand()
    {
        return VrSettings.LeftHandedMode.Value ? NonDominantHand : DominantHand;
    }

    private VrHand GetLeftHand()
    {
        return VrSettings.LeftHandedMode.Value ? DominantHand : NonDominantHand;
    }

    public VrHand GetMovementStickHand()
    {
        return VrSettings.SwapSticks.Value ? GetRightHand() : GetLeftHand();
    }

    private void HandleLeftHandedModeSettingChanged(object sender, EventArgs e)
    {
        UpdateHandedness();
    }

    private static Material GetArmsMaterial(Transform playerTransform)
    {
        return !playerTransform
            ? null
            : playerTransform.Find("henry/body")?.GetComponent<SkinnedMeshRenderer>().materials[2];
    }

    private void UpdateHandedness()
    {
        if (!henryTransform || !navigationController) return;

        var scale = new Vector3(VrSettings.LeftHandedMode.Value && navigationController.enabled ? -1 : 1, 1, 1);

        henryTransform.localScale = scale;

        var playerController = navigationController.playerController;

        if (playerController && playerController.inventory && playerController.inventory.heldObject)
            playerController.inventory.heldObject.transform.localScale = scale;
    }

    private Transform GetSkeletonRoot(Transform playerTransform)
    {
        if (playerTransform == null) return null;

        henryTransform = playerTransform.Find("henry");

        return henryTransform.Find("henryroot");
    }

    public void StopTrackingOriginalHands()
    {
        NonDominantHand.StopTrackingOriginalHands();
        DominantHand.StopTrackingOriginalHands();
    }

    public void StartTrackingOriginalHands()
    {
        NonDominantHand.StartTrackingOriginalHands();
        DominantHand.StartTrackingOriginalHands();
    }
}