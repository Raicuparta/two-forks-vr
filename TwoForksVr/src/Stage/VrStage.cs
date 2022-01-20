using System;
using TwoForksVr.Debugging;
using TwoForksVr.Helpers;
using TwoForksVr.Limbs;
using TwoForksVr.PlayerCamera;
using TwoForksVr.UI;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.Stage
{
    public class VrStage : MonoBehaviour
    {
        public static VrStage Instance;

        private VRCameraManager cameraManager;
        private FakeParenting follow;
        private VrLimbManager limbManager;
        private IntroFix introFix;
        private Camera mainCamera;
        private InteractiveUiTarget interactiveUiTarget;

        // No idea why, but if I don't make this static, it gets lost
        public static Camera FallbackCamera { get; private set; }

        private void Update()
        {
            if (!FallbackCamera.enabled && !(mainCamera && mainCamera.enabled)) SetUp(null, null);
        }

        private void OnDisable()
        {
            throw new Exception(
                "The VR Stage is being disabled. This should never happen. Check the call stack of this error to find the culprit.");
        }

        public static void Create(Transform parent)
        {
            if (Instance) return;
            var stageParent = new GameObject("VrStageParent")
            {
                // Apparently Firewatch will destroy all DontDrestroyOnLoad objects between scenes,
                // unless they have the MAIN tag.
                tag = "MAIN",
                transform = {parent = parent}
            };

            stageParent.AddComponent<vgOnlyLoadOnce>().dontDestroyOnLoad = true;

            DontDestroyOnLoad(stageParent);
            Instance = new GameObject("VrStage").AddComponent<VrStage>();
            Instance.transform.SetParent(stageParent.transform, false);
            Instance.cameraManager = VRCameraManager.Create(Instance);
            Instance.limbManager = VrLimbManager.Create(Instance);
            Instance.follow = stageParent.AddComponent<FakeParenting>();
            Instance.interactiveUiTarget = InteractiveUiTarget.Create(Instance);

            FallbackCamera = new GameObject("VrFallbackCamera").AddComponent<Camera>();
            FallbackCamera.enabled = false;
            FallbackCamera.clearFlags = CameraClearFlags.Color;
            FallbackCamera.backgroundColor = Color.black;
            FallbackCamera.transform.SetParent(Instance.transform, false);

            Instance.gameObject.AddComponent<GeneralDebugger>();
            
            return;
        }

        public void SetUp(Camera camera, Transform playerTransform)
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

            var nextCamera = mainCamera ? mainCamera : FallbackCamera;
            cameraManager.SetUp(nextCamera, playerTransform);
            limbManager.SetUp(playerTransform, nextCamera);
            interactiveUiTarget.SetUp(nextCamera);
        }

        public void Recenter(bool recenterVertically = false)
        {
            cameraManager.Recenter(recenterVertically);
        }

        public void HighlightButton(params ISteamVR_Action_In_Source[] actions)
        {
            limbManager.HighlightButton(actions);
        }
    }
}
