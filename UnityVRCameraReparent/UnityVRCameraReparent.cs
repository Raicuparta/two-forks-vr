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

        Transform rightHand;
        Transform leftHand;

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

                SetupCamera();
                ReparentCamera();
                var handPrefab = LoadHandPrefab();
                SetupHands(handPrefab);
            }

            if (rightHand)
            {
                rightHand.localPosition = InputTracking.GetLocalPosition(VRNode.RightHand);
                rightHand.localRotation = InputTracking.GetLocalRotation(VRNode.RightHand);
                leftHand.localPosition = InputTracking.GetLocalPosition(VRNode.LeftHand);
                leftHand.localRotation = InputTracking.GetLocalRotation(VRNode.LeftHand);
            }
        }

        private void SetupCamera()
        {
            var camera = Camera.main;
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.identity;
            camera.nearClipPlane = 0.03f;
            VRSettings.enabled = true;
        }

        private void SetupHands(GameObject prefab)
        {
            rightHand = CreateHand(prefab);
            leftHand = CreateHand(prefab, true);
        }

        private Transform CreateHand(GameObject prefab, bool isLeft = false)
        {
            var instance = UnityEngine.Object.Instantiate(prefab);
            var hand = instance.transform;
            hand.SetParent(Camera.main.transform.parent, false);
            var meshRenderer = GameObject.Find("Player Prefab").transform.Find("PlayerModel/henry/body").GetComponent<SkinnedMeshRenderer>();
            MelonLogger.Msg("after finding mesh renderer");
            hand.Find("hand").GetComponent<MeshRenderer>().material = meshRenderer.materials[2];

            if (isLeft)
            {
                hand.localScale = new Vector3(-hand.localScale.x, hand.localScale.y, hand.localScale.z);
            }

            return hand;
        }

        private GameObject LoadHandPrefab()
        {
            var myLoadedAssetBundle = AssetBundle.LoadFromFile(@"C:\Users\rai\Repos\FirewatchCode\Empty\FirewatchHelper\Assets\AssetBundles\hand");
            if (myLoadedAssetBundle == null)
            {
                MelonLogger.Error("Failed to load AssetBundle!");
                return null;
            }

            return myLoadedAssetBundle.LoadAsset<GameObject>("Hand");
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
