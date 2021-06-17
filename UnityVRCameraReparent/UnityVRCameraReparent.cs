using Harmony;
using MelonLoader;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.VR;

namespace Raicuparta.UnityVRCameraReparent
{
    public class UnityVRCameraReparent : MelonMod
    {

        Transform hand;
        Camera camera;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            HarmonyInstance.Create("Raicuparta.FirewatchVR").PatchAll();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            base.OnSceneWasInitialized(buildIndex, sceneName);

            MelonLogger.Msg("OnSceneWasInitialized");

            var cameraController = GameObject.FindObjectOfType<vgCameraController>();
            if (!cameraController)
            {
                return;
            }

            cameraController.defaultCameraTuning.ForEach(tuning => {
                tuning.minVerticalAngle = 0;
                tuning.maxVerticalAngle = 0;
            });
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Camera.main.enabled = false;
                camera = new GameObject().AddComponent<Camera>();
                camera.nearClipPlane = 0.0001f;
                camera.farClipPlane = 100f;
                LoadAssetBundle();
                //ReparentCamera();
            }

            if (hand)
            {
                hand.localPosition = InputTracking.GetLocalPosition(VRNode.RightHand);
                hand.localRotation = InputTracking.GetLocalRotation(VRNode.RightHand);
                //camera.transform.localPosition = InputTracking.GetLocalPosition(VRNode.CenterEye);
            }
        }

        private void LoadAssetBundle()
        {
            var myLoadedAssetBundle = AssetBundle.LoadFromFile(@"C:\Users\rai\Repos\FirewatchCode\Unity\globalgamemanagers\Assets\AssetBundles\hand");
            if (myLoadedAssetBundle == null)
            {
                MelonLogger.Error("Failed to load AssetBundle!");
                return;
            }


            var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("Hand");
            var instance = UnityEngine.Object.Instantiate(prefab);
            hand = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            hand.localScale = Vector3.one * 0.1f;
            //hand.SetParent(camera.transform, false);
            instance.transform.SetParent(hand, false);
            VRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
            //InputTracking.disablePositionalTracking = true;

            //var pose = instance.AddComponent<TrackedPoseDriver>();
            //pose.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.RightPose);
            //pose.UseRelativeTransform = false;
            //instance.transform.position = camera.transform.position + Vector3.forward;

        }

        private void ReparentCamera()
        {
            MelonLogger.Msg("Reparenting camera");

            var mainCamera = camera.transform;
            var vrCameraParent = new GameObject().transform;
            vrCameraParent.SetParent(mainCamera.parent, false);
            mainCamera.SetParent(vrCameraParent);
            vrCameraParent.localPosition = Vector3.down;
        }
    }

    [HarmonyPatch(typeof(vgCameraController), "LeanUp")]
    public class PatchLeanUp
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(vgCameraController), "LeanDown")]
    public class PatchLeanDown
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(vgCameraController), "LeanVertical")]
    public class PatchGetRoll
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            MelonLogger.Msg("Prefix");
            return false;
        }
    }
}
