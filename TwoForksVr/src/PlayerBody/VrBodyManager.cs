using System;
using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using UnityEngine;
using UnityEngine.Rendering;

namespace TwoForksVr.PlayerBody
{
    public class VrBodyManager : MonoBehaviour
    {
        private Material bodyMaterial;
        private Texture bodyTexture;

        private void Awake()
        {
            VrSettings.ShowFeet.SettingChanged += HandleSettingsChanged;
            VrSettings.ShowBody.SettingChanged += HandleSettingsChanged;
        }

        private void Start()
        {
            HideBodyParts();
        }

        private void OnDestroy()
        {
            VrSettings.ShowFeet.SettingChanged -= HandleSettingsChanged;
            VrSettings.ShowBody.SettingChanged -= HandleSettingsChanged;
        }

        public static void Create(vgPlayerController playerController)
        {
            var playerTransform = playerController.transform;
            var playerBody = playerTransform.Find("henry/body").gameObject;
            LayerHelper.SetLayer(playerBody, GameLayer.PlayerBody);
            var existingBodyManager = playerBody.GetComponent<VrBodyManager>();
            if (existingBodyManager) return;

            playerBody.AddComponent<VrBodyManager>();
        }

        // Hides body parts by either making them completely invisible,
        // or by using transparent textures to leave parts visible (hands and feet).
        private void HideBodyParts()
        {
            var renderer = transform.GetComponent<SkinnedMeshRenderer>();
            renderer.shadowCastingMode = ShadowCastingMode.TwoSided;
            
            var materials = renderer.materials;
            
            bodyMaterial = materials[0];
            bodyTexture = bodyMaterial.mainTexture;
            SetUpBodyVisibility();
            
            var backpackMaterial = materials[1];
            MakeMaterialTextureTransparent(backpackMaterial);
            
            var armsMaterial = materials[2];
            MakeMaterialTextureTransparent(armsMaterial);
        }

        private static void MakeMaterialTextureTransparent(Material material, Texture texture = null)
        {
            var cutoutShader = Shader.Find("Marmoset/Transparent/Cutout/Bumped Specular IBL");
            material.shader = cutoutShader;
            material.SetTexture(ShaderProperty.MainTexture, texture);
            if (!texture)
            {
                material.SetColor(ShaderProperty.Color, Color.clear);
            }
            else
            {
                material.SetColor(ShaderProperty.Color, Color.white);
            }
        }
        
        private void HandleSettingsChanged(object sender, EventArgs e)
        {
            SetUpBodyVisibility();
        }

        private Texture2D GetBodyTexture()
        {
            if (VrSettings.ShowBody.Value) return (Texture2D) bodyTexture;
            if (VrSettings.ShowFeet.Value) return VrAssetLoader.BodyCutoutTexture;
            return null;
        }

        private void SetUpBodyVisibility()
        {
            if (!bodyMaterial) return;
            MakeMaterialTextureTransparent(bodyMaterial, GetBodyTexture());
        }
    }
}