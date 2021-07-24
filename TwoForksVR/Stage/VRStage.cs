using MelonLoader;
using TwoForksVR.Hands;
using TwoForksVR.PlayerCamera;
using UnityEngine;

namespace TwoForksVR.Stage
{
    public class VRStage: MonoBehaviour
    {
        public static VRStage Instance;

        private VRCameraManager cameraManager;
        private VRHandsManager handsManager;
        private LateUpdateFollow follow;
        private Camera fallbackCamera;

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

                Instance.fallbackCamera = new GameObject("VR Camera").AddComponent<Camera>();
                Instance.fallbackCamera.enabled = false;
                Instance.fallbackCamera.clearFlags = CameraClearFlags.Color;
                Instance.fallbackCamera.backgroundColor = Color.black;
                Instance.fallbackCamera.tag = "MainCamera";
                Instance.fallbackCamera.transform.SetParent(Instance.transform, false);
            }
            return Instance;
        }

        public void SetUp(Camera camera, Transform playerTransform)
        {
            MelonLogger.Msg($"Setting up VRStage with camera {camera?.name} and player {playerTransform?.name}");

            if (camera.GetComponentInParent<VRStage>())
            {
                return;
            }

            var parent = camera?.transform.parent;

            //if (!parent)
            //{
            //    var existingStage = GameObject.Find("VRStage")?.GetComponent<VRStage>();
            //    if (existingStage) return;
            //}

            if (camera)
            {
                follow.Target = camera?.transform.parent;
            }
            else
            {
                MelonLogger.Msg("Setting up Intro stage");
                fallbackCamera.enabled = true;
                IntroFix.Create();
            }

            cameraManager.SetUp(camera ?? fallbackCamera);
            handsManager.SetUp(playerTransform);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Equals))
            {
                Time.timeScale = Time.timeScale > 1 ? 1 : 10;
            }
        }
    }
}
