using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Raicuparta.TwoForksVR
{
    public class VRHandManager: MonoBehaviour
    {
        public static Transform RightHand;
        public static Transform LeftHand;
        public Transform PlayerBody; // TODO get this some other way.

        private void Start()
        {
            var prefab = LoadHandPrefab();


            var handMaterial = GetHandMaterial();
            RightHand = CreateHand(prefab, handMaterial);
            LeftHand = CreateHand(prefab, handMaterial, true);
            SetUpLeftHandAttachment();
            SetUpRightHandAttachment();

            // Update pickupAttachTransform to hand.
            GameObject.FindObjectOfType<vgInventoryController>().CachePlayerVariables();
        }

        private Material GetHandMaterial()
        {
            return PlayerBody.GetComponent<SkinnedMeshRenderer>().materials[2];
        }

        private GameObject LoadHandPrefab()
        {
            var myLoadedAssetBundle = AssetBundle.LoadFromFile(Directory.GetCurrentDirectory() + "/Mods/TwoForksVR/hand");
            if (myLoadedAssetBundle == null)
            {
                MelonLogger.Error("Failed to load AssetBundle!");
                return null;
            }

            return myLoadedAssetBundle.LoadAsset<GameObject>("Hand");
        }

        private Transform CreateHand(GameObject prefab, Material material, bool isLeft = false)
        {
            var hand = Instantiate(prefab).AddComponent<VRHand>();
            hand.IsLeft = isLeft;
            hand.SetMaterial(material);

            return hand.transform;
        }

        private void SetUpLeftHandAttachment()
        {
            var itemSocket = LeftHand.Find("itemSocket");
            var handAttachment = GameObject.Find("henryHandLeftAttachment").transform;
            handAttachment.SetParent(itemSocket, false);
            itemSocket.localPosition = new Vector3(-0.08f, -0.06f, -0.056f);
            itemSocket.localEulerAngles = new Vector3(8.3794f, 341.5249f, 179.2709f);
        }

        private void SetUpRightHandAttachment()
        {
            var itemSocket = RightHand.Find("itemSocket");
            var handAttachment = GameObject.Find("henryHandRightAttachment").transform;
            handAttachment.SetParent(itemSocket, false);
            itemSocket.localScale = Vector3.one;
            itemSocket.localPosition = new Vector3(0.0551f, -0.0229f, -0.131f);
            itemSocket.localEulerAngles = new Vector3(54.1782f, 224.7767f, 139.0415f);
        }


        [HarmonyPatch(typeof(vgPlayerTargeting), "UpdateTarget")]
        public class PatchUpdateTarget
        {
            public static void Prefix(ref Vector3 cameraFacing, ref Vector3 cameraOrigin)
            {
                cameraFacing = RightHand.forward;
                cameraOrigin = RightHand.position;
            }
        }

        [HarmonyPatch(typeof(vgInventoryController), "CachePlayerVariables")]
        public class PatchCachePlayerVariables
        {
            public static void Postfix(ref Transform ___pickupAttachTransform)
            {
                ___pickupAttachTransform = RightHand;
            }
        }
    }
}
