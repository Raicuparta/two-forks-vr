using MelonLoader;
using TwoForksVR.Hands;
using TwoForksVR.PlayerCamera;
using UnityEngine;

namespace TwoForksVR.Stage
{
    public class VRStage: MonoBehaviour
    {
        public static VRStage Instance;

        // No idea why, but if I don't make this static, it gets lost
        public static Camera FallbackCamera { get; private set; }

        private VRCameraManager cameraManager;
        private VRHandsManager handsManager;
        private LateUpdateFollow follow;
        private Camera mainCamera;
        private IntroFix introFix;

        public static VRStage Create()
        {
            if (!Instance)
            {
                var stageParent = new GameObject("VRStageParent");

                // Apparently Firewatch will destroy all DontDrestroyOnLoad objects between scenes,
                // unless they have the MAIN tag.
                stageParent.tag = "MAIN";

                DontDestroyOnLoad(stageParent);
                Instance = new GameObject("VRStage").AddComponent<VRStage>();
                Instance.transform.SetParent(stageParent.transform, false);
                Instance.cameraManager = VRCameraManager.Create(Instance);
                Instance.handsManager = VRHandsManager.Create(Instance);
                Instance.follow = stageParent.AddComponent<LateUpdateFollow>();

                FallbackCamera = new GameObject("VRFallbackCamera").AddComponent<Camera>();
                FallbackCamera.enabled = false;
                FallbackCamera.clearFlags = CameraClearFlags.Color;
                FallbackCamera.backgroundColor = Color.black;
                FallbackCamera.transform.SetParent(Instance.transform, false);
            }
            return Instance;
        }

        public void SetUp(Camera camera, Transform playerTransform)
        {
            mainCamera = camera;
            follow.Target = mainCamera?.transform.parent;
            if (mainCamera)
            {
                FallbackCamera.enabled = false;
                VRStage.FallbackCamera.tag = "Untagged";
            }
            else
            {
                FallbackCamera.enabled = true;
                if (!introFix)
                {
                    introFix = IntroFix.Create();
                }
            }

            cameraManager.SetUp(mainCamera ?? FallbackCamera);
            handsManager.SetUp(playerTransform);
        }

        public void Recenter()
        {
            cameraManager.Recenter();
        }

        private void DelayedRecenter()
        {
            cameraManager.Recenter();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Equals))
            {
                Time.timeScale = Time.timeScale > 1 ? 1 : 10;
            }
            if (!FallbackCamera.enabled && !(mainCamera && mainCamera.enabled))
            {
                SetUp(null, null);
            }
        }
    }
}
