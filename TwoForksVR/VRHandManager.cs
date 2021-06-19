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
        public Transform PlayerBody; // TODO get this some other way.

        private static Transform LeftHand;
        private static Transform RightHand;

        private void Start()
        {
            var prefab = LoadHandPrefab();

            var handMaterial = GetHandMaterial();
            RightHand = CreateHand(prefab, handMaterial);
            LeftHand = CreateHand(prefab, handMaterial, true);
            SetUpHandAttachment(
                RightHand,
                "Right",
                new Vector3(0.0551f, -0.0229f, -0.131f),
                new Vector3(54.1782f, 224.7767f, 139.0415f)
            );
            SetUpHandAttachment(
                LeftHand,
                "Left",
                new Vector3(-0.08f, -0.06f, -0.056f),
                new Vector3(8.3794f, 341.5249f, 179.2709f)
            );

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

        private void SetUpHandAttachment(Transform hand, string handName, Vector3 position, Vector3 eulerAngles)
        {
            var itemSocket = hand.Find("itemSocket");
            var handAttachment = GameObject.Find($"henryHand{handName}Attachment").transform;
            handAttachment.SetParent(itemSocket, false);
            itemSocket.localPosition = position;
            itemSocket.localEulerAngles = eulerAngles;
        }

        [HarmonyPatch(typeof(vgInventoryController), "CachePlayerVariables")]
        public class PatchCachePlayerVariables
        {
            public static void Postfix(ref Transform ___pickupAttachTransform)
            {
                ___pickupAttachTransform = RightHand;
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
