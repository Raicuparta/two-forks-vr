using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwoForksVR.Hands;
using TwoForksVR.Assets;
using TwoForksVR.PlayerCamera;
using UnityEngine;

namespace TwoForksVR
{
    public class VRBodyManager: MonoBehaviour
    {
        private void Start()
        {
            var playerBodyTransform = GetPlayerBodyTransform();
            HideBody(playerBodyTransform);
            SetUpHandsManager(playerBodyTransform);
        }

        private void SetUpHandsManager(Transform playerBodyTransform)
        {
            var handsManager = new GameObject("VRHandsManager").AddComponent<VRHandsManager>(); // TODO use VRHands from asset
            handsManager.PlayerBody = playerBodyTransform;
            handsManager.transform.SetParent(Camera.main.transform.parent, false); // TODO use VR Stage
        }

        private void HideBody(Transform playerBodyTransform)
        {
            var renderer = playerBodyTransform.GetComponent<SkinnedMeshRenderer>();
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;

            var materials = renderer.materials;

            var bodyMaterial = materials[0];
            MakeMaterialTextureTransparent(bodyMaterial);

            var backpackMaterial = materials[1];
            MakeMaterialTextureTransparent(backpackMaterial);

            var armsMaterial = materials[2];
            MakeMaterialTextureTransparent(armsMaterial, VRAssetLoader.ArmsCutoutTexture);
        }

        private void MakeMaterialTextureTransparent(Material material, Texture2D texture = null)
        {
            var cutoutShader = Shader.Find("Marmoset/Transparent/Cutout/Bumped Specular IBL");
            material.shader = cutoutShader;
            material.SetTexture("_MainTex", texture);
            if (!texture)
            {
                material.SetColor("_Color", Color.clear);
            }
        }

        private Transform GetPlayerBodyTransform()
        {
            return GameObject.Find("Player Prefab").transform.Find("PlayerModel/henry/body");
        }
    }
}
