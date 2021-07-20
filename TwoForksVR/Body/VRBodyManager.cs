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
    public class VRBodyManager : MonoBehaviour
    {
        private static Transform playerBodyTransform;

        private void Start()
        {
            HideBody();
        }

        private void HideBody()
        {
            var renderer = GetPlayerBodyTransform().GetComponent<SkinnedMeshRenderer>();
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

        public static Transform GetPlayerBodyTransform()
        {
            return playerBodyTransform ?? (playerBodyTransform = GameObject.Find("Player Prefab")?.transform.Find("PlayerModel/henry/body"));
        }
    }
}
