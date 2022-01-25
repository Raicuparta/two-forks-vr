using System;
using TwoForksVr.Debugging;
using TwoForksVr.Helpers;
using TwoForksVr.Limbs;
using TwoForksVr.Locomotion;
using TwoForksVr.PlayerBody;
using TwoForksVr.Settings;
using TwoForksVr.UI;
using TwoForksVr.VrCamera;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.Stage
{
    public class VrStage : MonoBehaviour
    {
        private static VrStage instance;
        private BodyRendererManager bodyRendererManager;

        private VRCameraManager cameraManager;
        private FadeOverlay fadeOverlay;
        private FakeParenting follow;
        private InteractiveUiTarget interactiveUiTarget;
        private IntroFix introFix;
        private VrLimbManager limbManager;
        private Camera mainCamera;
        private RoomScaleBodyTransform roomScaleBodyTransform;
        private TeleportController teleportController;
        private TurningController turningController;
        private VeryLateUpdateManager veryLateUpdateManager;
        private VrSettingsMenu vrSettingsMenu;

        // No idea why, but if I don't make this static, it gets lost
        public static Camera FallbackCamera { get; private set; }

        public static void Create(Transform parent)
        {
            if (instance) return;
            var stageParent = new GameObject("VrStageParent")
            {
                // Apparently Firewatch will destroy all DontDrestroyOnLoad objects between scenes,
                // unless they have the MAIN tag.
                tag = "MAIN",
                transform = {parent = parent}
            };

            stageParent.AddComponent<vgOnlyLoadOnce>().dontDestroyOnLoad = true;

            DontDestroyOnLoad(stageParent);
            instance = new GameObject("VrStage").AddComponent<VrStage>();
            instance.transform.SetParent(stageParent.transform, false);
            instance.cameraManager = VRCameraManager.Create(instance);
            instance.limbManager = VrLimbManager.Create(instance);
            instance.follow = stageParent.AddComponent<FakeParenting>();
            instance.interactiveUiTarget = InteractiveUiTarget.Create(instance);
            instance.fadeOverlay = FadeOverlay.Create(instance);
            instance.teleportController = TeleportController.Create(instance, instance.limbManager);
            instance.veryLateUpdateManager = VeryLateUpdateManager.Create(instance);
            instance.turningController = TurningController.Create(instance, instance.teleportController);
            instance.roomScaleBodyTransform = RoomScaleBodyTransform.Create(instance, instance.teleportController);
            instance.bodyRendererManager = BodyRendererManager.Create(instance);
            instance.vrSettingsMenu = VrSettingsMenu.Create(instance);

            FallbackCamera = new GameObject("VrFallbackCamera").AddComponent<Camera>();
            FallbackCamera.enabled = false;
            FallbackCamera.clearFlags = CameraClearFlags.Color;
            FallbackCamera.backgroundColor = Color.black;
            FallbackCamera.transform.SetParent(instance.transform, false);

            instance.gameObject.AddComponent<GeneralDebugger>();

            TwoForksVrPatch.SetStage(instance);
        }

        public void SetUp(Camera camera, vgPlayerController playerController)
        {
            mainCamera = camera;
            if (mainCamera)
            {
                follow.Target = mainCamera.transform.parent;
                FallbackCamera.enabled = false;
                FallbackCamera.tag = "Untagged";
            }
            else
            {
                FallbackCamera.enabled = true;
                if (!introFix) introFix = IntroFix.Create();
            }

            var playerTransform = playerController ? playerController.transform : null;
            var nextCamera = mainCamera ? mainCamera : FallbackCamera;
            cameraManager.SetUp(nextCamera, playerTransform);
            limbManager.SetUp(playerTransform, nextCamera);
            interactiveUiTarget.SetUp(nextCamera);
            teleportController.SetUp(playerController);
            fadeOverlay.SetUp(nextCamera);
            veryLateUpdateManager.SetUp(nextCamera);
            turningController.SetUp(playerController);
            roomScaleBodyTransform.SetUp(playerController);
            bodyRendererManager.SetUp(playerController);
        }

        private void Update()
        {
            if (!FallbackCamera.enabled && !(mainCamera && mainCamera.enabled)) SetUp(null, null);
        }

        private void OnDisable()
        {
            throw new Exception(
                "The VR Stage is being disabled. This should never happen. Check the call stack of this error to find the culprit.");
        }

        public void RecenterPosition(bool recenterVertically = false)
        {
            cameraManager.RecenterPosition(recenterVertically);
        }

        public void RecenterRotation()
        {
            cameraManager.RecenterRotation();
        }

        public void HighlightButton(params ISteamVR_Action_In_Source[] actions)
        {
            limbManager.HighlightButton(actions);
        }

        public void FadeToBlack()
        {
            if (!fadeOverlay) return;
            fadeOverlay.FadeToBlack();
        }

        public void FadeToClear()
        {
            if (!fadeOverlay) return;
            fadeOverlay.FadeToClear();
        }

        public void OpenVrSettings()
        {
            if (!vrSettingsMenu) return;
            vrSettingsMenu.Open();
        }
    }
}