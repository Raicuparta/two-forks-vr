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

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            HarmonyInstance.Create("Raicuparta.FirewatchVR").PatchAll();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            VRSettings.enabled = false;
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

                var camera = Camera.main;
                camera.transform.localPosition = Vector3.zero;
                camera.transform.localRotation = Quaternion.identity;
                VRSettings.enabled = true;
                //camera.nearClipPlane = 0.0001f;
                //camera.farClipPlane = 100f;


                ReparentCamera();
                LoadAssetBundle();
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
            var myLoadedAssetBundle = AssetBundle.LoadFromFile(@"C:\Users\rai\Repos\FirewatchCode\Empty\FirewatchHelper\Assets\AssetBundles\hand");
            if (myLoadedAssetBundle == null)
            {
                MelonLogger.Error("Failed to load AssetBundle!");
                return;
            }


            var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("Hand");
            var instance = UnityEngine.Object.Instantiate(prefab);
            hand = instance.transform;
            //hand.localScale = Vector3.one * 0.05f;
            hand.SetParent(Camera.main.transform.parent, false);
            //hand.Find("hand").GetComponent<MeshRenderer>().materials = GameObject.FindObjectOfType<vgPlayerController>().GetComponentInChildren<SkinnedMeshRenderer>().materials;
            var meshRenderer = GameObject.Find("Player Prefab").transform.Find("PlayerModel/henry/body").GetComponent<SkinnedMeshRenderer>();
            MelonLogger.Msg("after finding mesh renderer");
            hand.Find("hand").GetComponent<MeshRenderer>().material = meshRenderer.materials[2];
            //instance.transform.SetParent(hand, false);
            //VRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
            //InputTracking.disablePositionalTracking = true;

            //var pose = instance.AddComponent<TrackedPoseDriver>();
            //pose.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.RightPose);
            //pose.UseRelativeTransform = false;
            //instance.transform.position = camera.transform.position + Vector3.forward;

        }

        private void ReparentCamera()
        {
            MelonLogger.Msg("Reparenting camera");

            var mainCamera = Camera.main.transform;
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
