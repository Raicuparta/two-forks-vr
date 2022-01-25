using System;
using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using UnityEngine;
using UnityEngine.Rendering;

namespace TwoForksVr.PlayerBody
{
    public class VrBodyManager: MonoBehaviour
    {
        private Material bodyMaterial;
        private Texture bodyTexture;
        private SkinnedMeshRenderer renderer;

        private void Awake()
        {
            VrSettings.ShowFeet.SettingChanged += HandleSettingsChanged;
            VrSettings.ShowBody.SettingChanged += HandleSettingsChanged;
        }

        private void OnDestroy()
        {
            VrSettings.ShowFeet.SettingChanged -= HandleSettingsChanged;
            VrSettings.ShowBody.SettingChanged -= HandleSettingsChanged;
        }

        public static VrBodyManager Create(VrStage stage)
        {
            var instance = stage.gameObject.AddComponent<VrBodyManager>();
            return instance;
        }
        
        public void SetUp(vgPlayerController playerController)
        {
            if (!playerController) return;
            var playerBody = playerController.transform.Find("henry/body").gameObject;
            renderer = playerBody.GetComponent<SkinnedMeshRenderer>();
            LayerHelper.SetLayer(playerBody, GameLayer.PlayerBody);
            
            HideBodyParts();
        }

        // Hides body parts by either making them completely invisible,
        // or by using transparent textures to leave parts visible (hands and feet).
        private void HideBodyParts()
        {
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
            material.SetColor(ShaderProperty.Color, texture ? Color.white : Color.clear);
        }
        
        private void HandleSettingsChanged(object sender, EventArgs e)
        {
            SetUpBodyVisibility();
        }

        private Texture2D GetBodyTexture()
        {
            if (VrSettings.ShowBody.Value) return (Texture2D) bodyTexture;
            return VrSettings.ShowFeet.Value ? VrAssetLoader.BodyCutoutTexture : null;
        }

        private void SetUpBodyVisibility()
        {
            if (!bodyMaterial) return;
            MakeMaterialTextureTransparent(bodyMaterial, GetBodyTexture());
        }
    }
}