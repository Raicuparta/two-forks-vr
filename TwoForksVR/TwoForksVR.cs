using Harmony;
using MelonLoader;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.VR;
using System.Linq;

namespace Raicuparta.TwoForksVR
{
    public class TwoForksVR : MelonMod
    {
        Transform playerBody;
        static bool isVrInitialized;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            HarmonyInstance.Create("Raicuparta.FirewatchVR").PatchAll();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (!isVrInitialized)
            {
                VRSettings.enabled = false;
            }
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
                SetUpUI();
                var handManager = new GameObject().AddComponent<VRHandManager>();
                handManager.PlayerBody = playerBody;
            }
        }

        private void SetUpPlayerBody()
        {
            playerBody = GameObject.Find("Player Prefab").transform.Find("PlayerModel/henry/body");
            playerBody.gameObject.SetActive(false);
        }

        private void SetUpCamera()
        {
            var camera = Camera.main;
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localRotation = Quaternion.identity;
            camera.nearClipPlane = 0.03f;
            if (!isVrInitialized)
            {
                VRSettings.enabled = true;
                isVrInitialized = true;
            }
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



        private void ReparentCamera()
        {
            var mainCamera = Camera.main.transform;
            var vrCameraParent = new GameObject("VR Stage").transform;
            vrCameraParent.SetParent(mainCamera.parent, false);
            mainCamera.SetParent(vrCameraParent);
            vrCameraParent.localPosition = Vector3.down * 1.2f;
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

        [HarmonyPatch(typeof(vgCompass), "LateUpdate")]
        public class PatchCompass
        {
            public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
            {
                float unsignedAngle = Vector3.Angle(from, to);

                float cross_x = from.y * to.z - from.z * to.y;
                float cross_y = from.z * to.x - from.x * to.z;
                float cross_z = from.x * to.y - from.y * to.x;
                float sign = Mathf.Sign(axis.x * cross_x + axis.y * cross_y + axis.z * cross_z);
                return unsignedAngle * sign;
            }

            public static bool Prefix(vgCompass __instance, Vector3 ___newRotation, float ___worldOffset)
            {
                var transform = __instance.transform;
                var forward = Vector3.ProjectOnPlane(-transform.parent.forward, Vector3.up);
                var angle = SignedAngle(forward, Vector3.forward, Vector3.up);
                ___newRotation.y = angle - 165f - ___worldOffset;
                transform.localEulerAngles = ___newRotation;
                return false;
            }
        }

        [HarmonyPatch(typeof(vgCompass), "Start")]
        public class PatchCompassStart
        {
            private static void CreateLine(Color color, Transform parent, Vector3 destination)
            {
                var line = new GameObject("debugLine").transform;
                line.transform.SetParent(parent, false);

                var lineRenderer = line.gameObject.AddComponent<LineRenderer>();
                lineRenderer.useWorldSpace = false;
                lineRenderer.SetPositions(new[] { Vector3.zero, destination });
                lineRenderer.startWidth = 0.005f;
                lineRenderer.endWidth = 0.005f;
                lineRenderer.endColor = color;
                lineRenderer.startColor = color;
                lineRenderer.material.shader = Shader.Find("Particles/Alpha Blended Premultiply");
                lineRenderer.material.SetColor("_Color", color);
            }

            public static void Postfix(vgCompass __instance)
            {
                var transform = __instance.transform.parent;
                CreateLine(Color.red, transform, Vector3.right);
                CreateLine(Color.green, transform, Vector3.forward);
                CreateLine(Color.blue, transform, Vector3.up);
            }
        }
    }
}
