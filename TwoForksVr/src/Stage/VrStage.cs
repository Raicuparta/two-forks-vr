using System;
using System.Linq;
using TwoForksVr.Debugging;
using TwoForksVr.Helpers;
using TwoForksVr.Limbs;
using TwoForksVr.Liv;
using TwoForksVr.Locomotion;
using TwoForksVr.PlayerBody;
using TwoForksVr.Settings;
using TwoForksVr.UI;
using TwoForksVr.VrCamera;
using TwoForksVr.VrInput;
using TwoForksVr.VrInput.ActionInputs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TwoForksVr.Stage;

public class VrStage : MonoBehaviour
{
    private static VrStage instance;

    private static readonly string[] fallbackCameraTagSkipScenes = {"Main", "PreLoad"};
    private BindingsManager bindingsManager;
    private BodyRendererManager bodyRendererManager;

    private VRCameraManager cameraManager;

    // private FadeOverlay fadeOverlay;
    private Camera fallbackCamera;
    private FakeParenting follow;
    private InteractiveUiTarget interactiveUiTarget;
    private IntroFix introFix;
    private VrLimbManager limbManager;
    private Camera mainCamera;
    private RoomScaleBodyTransform roomScaleBodyTransform;
    private StaticUiTarget staticUiTarget;
    private TeleportController teleportController;
    private TurningController turningController;
    private VeryLateUpdateManager veryLateUpdateManager;
    private VrSettingsMenu vrSettingsMenu;
    
    // TODO: temporarily disabling LIV since it's causing problems.
    // private LivManager livManager;

    public static void Create(Transform parent)
    {
        if (instance) return;
        var stageParent = new GameObject("VrStageParent")
        {
            // Apparently Firewatch will destroy all DontDestroyOnLoad objects between scenes,
            // unless they have the MAIN tag.
            tag = GameTag.Main,
            transform = {parent = parent}
        };

        stageParent.AddComponent<vgOnlyLoadOnce>().dontDestroyOnLoad = true;

        DontDestroyOnLoad(stageParent);
        instance = new GameObject("VrStage").AddComponent<VrStage>();
        instance.transform.SetParent(stageParent.transform, false);
        instance.cameraManager = VRCameraManager.Create(instance);
        instance.limbManager = VrLimbManager.Create(instance);
        instance.follow = stageParent.AddComponent<FakeParenting>();
        instance.follow = FakeParenting.Create(stageParent.transform);
        instance.interactiveUiTarget = InteractiveUiTarget.Create(instance);
        instance.staticUiTarget = StaticUiTarget.Create(instance);
        instance.teleportController = TeleportController.Create(instance, instance.limbManager);
        instance.turningController =
            TurningController.Create(instance, instance.teleportController, instance.limbManager);
        instance.veryLateUpdateManager = VeryLateUpdateManager.Create(instance);
        instance.roomScaleBodyTransform = RoomScaleBodyTransform.Create(instance, instance.teleportController);
        instance.bodyRendererManager =
            BodyRendererManager.Create(instance, instance.teleportController, instance.limbManager);
        instance.vrSettingsMenu = VrSettingsMenu.Create(instance);
        instance.bindingsManager = BindingsManager.Create(instance);
        
        // TODO: temporarily disabling LIV since it's causing problems.
        // instance.livManager = LivManager.Create(instance);

        instance.fallbackCamera = new GameObject("VrFallbackCamera").AddComponent<Camera>();
        instance.fallbackCamera.enabled = false;
        instance.fallbackCamera.clearFlags = CameraClearFlags.Color;
        instance.fallbackCamera.backgroundColor = Color.black;
        instance.fallbackCamera.transform.SetParent(instance.transform, false);

        instance.gameObject.AddComponent<GeneralDebugger>();

        TwoForksVrPatch.SetStage(instance);
    }

    public void SetUp(Camera camera, vgPlayerController playerController)
    {
        mainCamera = camera;
        if (mainCamera)
        {
            follow.SetTarget(mainCamera.transform.parent);
            fallbackCamera.enabled = false;
            fallbackCamera.tag = GameTag.Untagged;
        }
        else
        {
            fallbackCamera.enabled = true;
            if (!fallbackCameraTagSkipScenes.Contains(SceneManager.GetActiveScene().name))
                fallbackCamera.tag = GameTag.MainCamera;
            if (!introFix) introFix = IntroFix.Create();
        }

        var playerTransform = playerController ? playerController.transform : null;
        var nextCamera = mainCamera ? mainCamera : fallbackCamera;
        cameraManager.SetUp(nextCamera, playerTransform);
        limbManager.SetUp(playerController, nextCamera);
        interactiveUiTarget.SetUp(nextCamera);
        staticUiTarget.SetUp(nextCamera);
        teleportController.SetUp(playerController);
        FadeOverlay.Create(nextCamera);
        veryLateUpdateManager.SetUp(nextCamera);
        turningController.SetUp(playerController);
        roomScaleBodyTransform.SetUp(playerController);
        bodyRendererManager.SetUp(playerController);
        
        // TODO: temporarily disabling LIV since it's causing problems.
        // livManager.SetUp(nextCamera);
    }

    private void Update()
    {
        if (!fallbackCamera.enabled && !(mainCamera && mainCamera.enabled)) SetUp(null, null);
    }

    private void OnDisable()
    {
        throw new Exception(
            "The VR Stage is being disabled. This should never happen. Check the call stack of this error to find the culprit.");
    }

    public Camera GetMainCamera()
    {
        return mainCamera;
    }

    public void RecenterPosition(bool recenterVertically = false)
    {
        cameraManager.RecenterPosition(recenterVertically);
    }

    public void RecenterRotation()
    {
        cameraManager.RecenterRotation();
    }

    public void Recenter()
    {
        RecenterPosition(true);
        RecenterRotation();
    }

    public void FadeToBlack()
    {
        FadeOverlay.StartFade(Color.black, FadeOverlay.Duration);
    }

    public void FadeToClear()
    {
        FadeOverlay.StartFade(Color.clear, FadeOverlay.Duration);
    }

    public void OpenVrSettings()
    {
        if (!vrSettingsMenu) return;
        vrSettingsMenu.Open();
    }

    public bool IsTeleporting()
    {
        return teleportController && teleportController.IsTeleporting();
    }

    public bool IsNextToTeleportMarker(Transform playerControllerTransform)
    {
        return teleportController.IsNextToTeleportMarker(playerControllerTransform);
    }

    public Transform GetInteractiveUiTarget()
    {
        return interactiveUiTarget ? interactiveUiTarget.TargetTransform : null;
    }

    public Transform GetStaticUiTarget()
    {
        return staticUiTarget ? staticUiTarget.TargetTransform : null;
    }

    public IActionInput GetInputAction(string virtualKey)
    {
        if (!bindingsManager) return null;
        bindingsManager.ActionMap.TryGetValue(virtualKey, out var value);
        return value;
    }

    public float GetInputValue(string virtualKey)
    {
        if (!bindingsManager) return 0;
        return bindingsManager.GetValue(virtualKey);
    }

    public bool GetInputUp(string virtualKey)
    {
        return bindingsManager && bindingsManager.GetUp(virtualKey);
    }

    public bool GetInputDown(string virtualKey)
    {
        return bindingsManager && bindingsManager.GetDown(virtualKey);
    }

    public Transform GetLaserTransform()
    {
        if (limbManager == null || limbManager.Laser == null) return null;
        return limbManager.Laser.transform;
    }

    public Transform GetDominantHand()
    {
        return limbManager == null ? null : limbManager.DominantHand.transform;
    }


    public Transform GetMovementStickHand()
    {
        return limbManager == null ? null : limbManager.GetMovementStickHand().transform;
    }
}