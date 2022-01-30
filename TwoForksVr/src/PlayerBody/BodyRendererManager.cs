using System;
using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.Locomotion;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using UnityEngine;
using UnityEngine.Rendering;

namespace TwoForksVr.PlayerBody
{
    public class BodyRendererManager : MonoBehaviour
    {
        private Material backpackMaterial;
        private Texture backpackTexture;
        private Material bodyMaterial;
        private Texture bodyTexture;
        private Shader cutoutShader;
        private bool isShowingFullBody;
        private vgPlayerNavigationController navigationController;
        private SkinnedMeshRenderer renderer;
        private TeleportController teleportController;

        public static BodyRendererManager Create(VrStage stage, TeleportController teleportController)
        {
            var instance = stage.gameObject.AddComponent<BodyRendererManager>();
            instance.teleportController = teleportController;
            return instance;
        }

        public void SetUp(vgPlayerController playerController)
        {
            if (!playerController) return;
            var playerBody = playerController.transform.Find("henry/body").gameObject;
            renderer = playerBody.GetComponent<SkinnedMeshRenderer>();
            LayerHelper.SetLayer(playerBody, GameLayer.PlayerBody);
            navigationController = playerController.navController;

            HideBodyParts();
        }

        private void Awake()
        {
            VrSettings.Config.SettingChanged += HandleSettingsChanged;
            cutoutShader = Shader.Find("Marmoset/Transparent/Cutout/Bumped Specular IBL");
        }

        private void Update()
        {
            var shouldShowFullBody = ShouldShowFullBody();
            if (!isShowingFullBody && shouldShowFullBody)
                isShowingFullBody = true;
            else if (isShowingFullBody && !shouldShowFullBody)
                isShowingFullBody = false;
            else
                return;
            SetVisibilityAcordingToState();
        }

        private void OnDestroy()
        {
            VrSettings.Config.SettingChanged -= HandleSettingsChanged;
        }

        private bool ShouldShowFullBody()
        {
            return teleportController.IsTeleporting() ||
                   VrSettings.FixedCameraDuringAnimations.Value && navigationController &&
                   !navigationController.enabled;
        }

        // Hides body parts by either making them completely invisible,
        // or by using transparent textures to leave parts visible (hands and feet).
        private void HideBodyParts()
        {
            renderer.shadowCastingMode = ShadowCastingMode.TwoSided;

            var materials = renderer.materials;

            bodyMaterial = materials[0];
            bodyTexture = bodyMaterial.mainTexture;

            backpackMaterial = materials[1];
            backpackTexture = backpackMaterial.mainTexture;

            var armsMaterial = materials[2];
            MakeMaterialTextureTransparent(armsMaterial);

            SetVisibilityAcordingToState();
        }

        private void MakeMaterialTextureTransparent(Material material, Texture texture = null)
        {
            material.shader = cutoutShader;
            material.SetTexture(ShaderProperty.MainTexture, texture);
            material.SetColor(ShaderProperty.Color, texture ? Color.white : Color.clear);
        }

        private void HandleSettingsChanged(object sender, EventArgs e)
        {
            SetVisibilityAcordingToState();
        }

        private Texture2D GetBodyTexture()
        {
            if (VrSettings.ShowBody.Value || isShowingFullBody) return (Texture2D) bodyTexture;
            return VrSettings.ShowFeet.Value ? VrAssetLoader.BodyCutoutTexture : null;
        }

        private Texture2D GetBackpackTexture()
        {
            return isShowingFullBody ? (Texture2D) backpackTexture : null;
        }

        private void SetVisibilityAcordingToState()
        {
            MakeMaterialTextureTransparent(bodyMaterial, GetBodyTexture());
            MakeMaterialTextureTransparent(backpackMaterial, GetBackpackTexture());
        }
    }
}