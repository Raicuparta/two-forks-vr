using Harmony;
using MelonLoader;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.VR;
using System.Linq;

namespace Raicuparta.UnityVRCameraReparent
{
    public class UnityVRCameraReparent : MelonMod
    {

        static Transform rightHand;
        static Transform leftHand;
        Transform playerBody;

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
                SetUpPlayerBody();
                SetUpCamera();
                ReparentCamera();
                var handPrefab = LoadHandPrefab();
                SetUpHands(handPrefab);
                SetUpUI();
                SetUpHandLaser();
                SetUpLeftHandAttachment();
            }

            if (rightHand)
            {
                rightHand.localPosition = InputTracking.GetLocalPosition(VRNode.RightHand);
                rightHand.localRotation = InputTracking.GetLocalRotation(VRNode.RightHand);
                leftHand.localPosition = InputTracking.GetLocalPosition(VRNode.LeftHand);
                leftHand.localRotation = InputTracking.GetLocalRotation(VRNode.LeftHand);
            }
        }

        private void SetUpPlayerBody()
        {
            playerBody = GameObject.Find("Player Prefab").transform.Find("PlayerModel/henry/body");
            playerBody.gameObject.SetActive(false);
        }

        private void SetUpLeftHandAttachment()
        {
            GameObject.Find("henryHandLeftAttachment").transform.SetParent(leftHand, false);
        }

        private void SetUpCamera()
        {
            var camera = Camera.main;
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.identity;
            camera.nearClipPlane = 0.03f;
            VRSettings.enabled = true;
        }

        private void SetUpUI()
        {
            var canvases = GameObject.FindObjectsOfType<Canvas>().Where(canvas => canvas.renderMode == RenderMode.ScreenSpaceOverlay);
            canvases.Do(canvas =>
            {
                if (canvas.name == "BlackBars")
                {
                    canvas.enabled = false;
                    return;
                }
                canvas.worldCamera = Camera.main;
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.transform.SetParent(Camera.main.transform, false);
                canvas.transform.localPosition = Vector3.forward * 0.5f;
                canvas.transform.localScale = Vector3.one * 0.0004f;
            });
        }

        private void SetUpWeddingRing(Transform hand)
        {
            var weddingRing = GameObject.Find("HenryWeddingRing 1").transform;
            var socket = hand.Find("weddingRingSocket");
            weddingRing.SetParent(socket);
            weddingRing.localPosition = Vector3.zero;
            weddingRing.localRotation = Quaternion.identity;
        }

        private void SetUpHands(GameObject prefab)
        {
            rightHand = CreateHand(prefab);
            leftHand = CreateHand(prefab, true);

            // Update pickupAttachTransform to hand.
            GameObject.FindObjectOfType<vgInventoryController>().CachePlayerVariables();
            
        }

        private Transform CreateHand(GameObject prefab, bool isLeft = false)
        {
            var instance = UnityEngine.Object.Instantiate(prefab);
            instance.name = isLeft ? "Left VR Hand" : "Right VR Hand";
            var hand = instance.transform;
            hand.SetParent(Camera.main.transform.parent, false);
            var meshRenderer = playerBody.GetComponent<SkinnedMeshRenderer>();
            MelonLogger.Msg("after finding mesh renderer");
            hand.Find("hand").GetComponent<MeshRenderer>().material = meshRenderer.materials[2];

            if (isLeft)
            {
                hand.localScale = new Vector3(-hand.localScale.x, hand.localScale.y, hand.localScale.z);
                SetUpWeddingRing(hand);
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
            var vrCameraParent = new GameObject("VR Stage").transform;
            vrCameraParent.SetParent(mainCamera.parent, false);
            mainCamera.SetParent(vrCameraParent);
            vrCameraParent.localPosition = Vector3.down * 1.2f;
        }

        private void SetUpHandLaser()
        {
            var laser = new GameObject("VR Laser").transform;
            laser.transform.SetParent(rightHand, false);
            //laser.localPosition = new Vector3(0f, -0.05f, 0.01f);
            //laser.localRotation = Quaternion.Euler(45f, 0, 0);

            var lineRenderer = laser.gameObject.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.SetPositions(new[] { Vector3.zero, Vector3.forward });
            lineRenderer.startWidth = 0.005f;
            lineRenderer.endWidth = 0.001f;
            lineRenderer.endColor = new Color(1, 1, 1, 0.3f);
            lineRenderer.startColor = Color.clear;
            lineRenderer.material.shader = Shader.Find("Particles/Alpha Blended Premultiply");
            lineRenderer.material.SetColor("_Color", new Color(0.8f, 0.8f, 0.8f));
        }

        [HarmonyPatch(typeof(vgPlayerTargeting), "UpdateTarget")]
        public class PatchUpdateTarget
        {
            public static void Prefix(ref Vector3 cameraFacing, ref Vector3 cameraOrigin)
            {
                cameraFacing = rightHand.forward;
                cameraOrigin = rightHand.position;
            }
        }

        [HarmonyPatch(typeof(vgInventoryController), "CachePlayerVariables")]
        public class PatchCachePlayerVariables
        {
            public static void Postfix(ref Transform ___pickupAttachTransform)
            {
                ___pickupAttachTransform = rightHand;
            }
        }

        [HarmonyPatch(typeof(vgInventoryController), "TossStart")]
        public class PatchTossAnimation
        {
            public static bool Prefix(vgInventoryController __instance)
            {
                __instance.OnToss();
                return false;
            }
        }
    }
}
