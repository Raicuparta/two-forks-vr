using System;
using System.Linq;
using TwoForksVr.Debugging;
using TwoForksVr.Helpers;
using TwoForksVr.Limbs;
using TwoForksVr.Locomotion;
using TwoForksVr.PlayerBody;
using TwoForksVr.Settings;
using TwoForksVr.UI;
using TwoForksVr.VrCamera;
using TwoForksVr.VrInput;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

namespace TwoForksVr.Stage
{
    public class VrStage : MonoBehaviour
    {
        private static VrStage instance;

        private static readonly string[] setupSkippingScenes = {"Main", "PreLoad"};
        private BindingsManager bindingsManager;
        private BodyRendererManager bodyRendererManager;

        private VRCameraManager cameraManager;
        private FadeOverlay fadeOverlay;
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

        public static void Create(Transform parent)
        {
            if (instance) return;
            var stageParent = new GameObject("VrStageParent")
            {
                // Apparently Firewatch will destroy all DontDrestroyOnLoad objects between scenes,
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
            instance.interactiveUiTarget = InteractiveUiTarget.Create(instance);
            instance.staticUiTarget = StaticUiTarget.Create(instance);
            instance.fadeOverlay = FadeOverlay.Create(instance);
            instance.teleportController = TeleportController.Create(instance, instance.limbManager);
            instance.veryLateUpdateManager = VeryLateUpdateManager.Create(instance);
            instance.turningController = TurningController.Create(instance, instance.teleportController);
            instance.roomScaleBodyTransform = RoomScaleBodyTransform.Create(instance, instance.teleportController);
            instance.bodyRendererManager = BodyRendererManager.Create(instance, instance.teleportController);
            instance.vrSettingsMenu = VrSettingsMenu.Create(instance);
            instance.bindingsManager = BindingsManager.Create(instance);

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
            if (setupSkippingScenes.Contains(SceneManager.GetActiveScene().name)) return;

            mainCamera = camera;
            if (mainCamera)
            {
                follow.Target = mainCamera.transform.parent;
                fallbackCamera.enabled = false;
                fallbackCamera.tag = GameTag.Untagged;
            }
            else if (!Camera.main)
            {
                fallbackCamera.enabled = true;
                fallbackCamera.tag = GameTag.MainCamera;
                if (!introFix) introFix = IntroFix.Create();
            }

            var playerTransform = playerController ? playerController.transform : null;
            var nextCamera = mainCamera ? mainCamera : fallbackCamera;
            cameraManager.SetUp(nextCamera, playerTransform);
            limbManager.SetUp(playerTransform, nextCamera);
            interactiveUiTarget.SetUp(nextCamera);
            staticUiTarget.SetUp(nextCamera);
            teleportController.SetUp(playerController);
            fadeOverlay.SetUp(nextCamera);
            veryLateUpdateManager.SetUp(nextCamera);
            turningController.SetUp(playerController);
            roomScaleBodyTransform.SetUp(playerController);
            bodyRendererManager.SetUp(playerController);
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

        public Camera GetActiveCamera()
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

        public bool IsVector2CommandExisting(string command)
        {
            if (!bindingsManager) return false;

            return bindingsManager.Vector2XActionMap.ContainsKey(command) ||
                   bindingsManager.Vector2YActionMap.ContainsKey(command);
        }

        public SteamVR_Action_Boolean GetBooleanAction(string command)
        {
            bindingsManager.BooleanActionMap.TryGetValue(command, out var value);
            return value;
        }
    }
}