using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwoForksVR.Hands;
using UnityEngine;

namespace TwoForksVR
{
    public class VRBodyManager: MonoBehaviour
    {
        private void Start()
        {
            var playerBodyTransform = GetPlayerBodyTransform();
            HideBody(playerBodyTransform);

            var handsManager = new GameObject("VRHandsManager").AddComponent<VRHandsManager>();
            handsManager.PlayerBody = playerBodyTransform;
        }

        private void HideBody(Transform bodyTransform)
        {
            var materials = bodyTransform.GetComponent<SkinnedMeshRenderer>().materials;

            var bodyMaterial = materials[0];
            MakeMaterialTextureTransparent(bodyMaterial);

            var backpackMaterial = materials[1];
            MakeMaterialTextureTransparent(backpackMaterial);

            var armsMaterial = materials[2];
            // TODO arms texture replace.
        }

        private void MakeMaterialTextureTransparent(Material material)
        {
            var cutoutShader = Shader.Find("Marmoset/Transparent/Cutout/Bumped Specular IBL");
            material.shader = cutoutShader;
            material.SetTexture("_MainTex", null);
            material.SetColor("_Color", Color.clear);
        }

        private Transform GetPlayerBodyTransform()
        {
            return GameObject.Find("Player Prefab").transform.Find("PlayerModel/henry/body");
        }
    }
}
