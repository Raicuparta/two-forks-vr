using System;
using TwoForksVr.Debugging;
using TwoForksVr.Helpers;
using TwoForksVr.Limbs;
using TwoForksVr.PlayerCamera;
using UnityEngine;

namespace TwoForksVr.Stage
{
    public class VRStage : MonoBehaviour
    {
        public static VRStage Instance;

        private VRCameraManager cameraManager;
        private LateUpdateFollow follow;
        private VrLimbManager limbManager;
        private IntroFix introFix;
        private Camera mainCamera;

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

        public static VRStage Create(Transform parent)
        {
            if (Instance) return Instance;
            var stageParent = new GameObject("VRStageParent")
            {
                // Apparently Firewatch will destroy all DontDrestroyOnLoad objects between scenes,
                // unless they have the MAIN tag.
                tag = "MAIN",
                transform = {parent = parent}
            };

            stageParent.AddComponent<vgOnlyLoadOnce>().dontDestroyOnLoad = true;

            DontDestroyOnLoad(stageParent);
            Instance = new GameObject("VRStage").AddComponent<VRStage>();
            Instance.transform.SetParent(stageParent.transform, false);
            Instance.cameraManager = VRCameraManager.Create(Instance);
            Instance.limbManager = VrLimbManager.Create(Instance);
            Instance.follow = stageParent.AddComponent<LateUpdateFollow>();

            FallbackCamera = new GameObject("VRFallbackCamera").AddComponent<Camera>();
            FallbackCamera.enabled = false;
            FallbackCamera.clearFlags = CameraClearFlags.Color;
            FallbackCamera.backgroundColor = Color.black;
            FallbackCamera.transform.SetParent(Instance.transform, false);

            Instance.gameObject.AddComponent<GeneralDebugger>();

            return Instance;
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

            cameraManager.SetUp(mainCamera ? mainCamera : FallbackCamera);
            limbManager.SetUp(playerTransform);
        }

        public void Recenter(bool recenterVertically = false)
        {
            cameraManager.Recenter(recenterVertically);
        }
    }
}
