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
        private Material armsMaterial;
        private Material backpackMaterial;
        private Texture backpackTexture;
        private Material bodyMaterial;
        private Texture bodyTexture;
        private Shader cutoutShader;
        private bool isShowingArms;
        private bool isShowingFullBody;
        private vgPlayerNavigationController navigationController;
        private SkinnedMeshRenderer renderer;
        private TeleportController teleportController;
        private Shader transparentShader;

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
            transparentShader = Shader.Find("Marmoset/Transparent/Effects/Diffuse IBL Additive");
            cutoutShader = Shader.Find("Marmoset/Transparent/Cutout/Bumped Specular IBL");
        }

        private void Update()
        {
            UpdateShowFullBody();
            UpdateShowArms();
        }

        private void OnDestroy()
        {
            VrSettings.Config.SettingChanged -= HandleSettingsChanged;
        }

        private void UpdateShowFullBody()
        {
            var shouldShowFullBody = ShouldShowFullBody();
            if (!isShowingFullBody && shouldShowFullBody)
                isShowingFullBody = true;
            else if (isShowingFullBody && !shouldShowFullBody)
                isShowingFullBody = false;
            else
                return;
            SetBodyVisibilityAccordingToState();
        }

        private void UpdateShowArms()
        {
            var shoudlShowArms = ShouldShowArms();
            if (!isShowingArms && shoudlShowArms)
                isShowingArms = true;
            else if (isShowingArms && !shoudlShowArms)
                isShowingArms = false;
            else
                return;
            SetArmsVisibilityAccordingToState();
        }

        private bool ShouldShowFullBody()
        {
            return teleportController.IsTeleporting() ||
                   VrSettings.FixedCameraDuringAnimations.Value && !IsNavigationControllerEnabled();
        }

        private bool IsNavigationControllerEnabled()
        {
            return navigationController && navigationController.enabled;
        }

        private bool ShouldShowArms()
        {
            // TODO add setting for this
            return !teleportController.IsTeleporting() && !IsNavigationControllerEnabled();
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

            armsMaterial = materials[2];

            SetBodyVisibilityAccordingToState();
            SetArmsVisibilityAccordingToState();
        }

        private void MakeMaterialTextureTransparent(Material material, Texture texture = null)
        {
            if (!material) return;
            material.shader = texture ? transparentShader : cutoutShader;
            material.SetTexture(ShaderProperty.MainTexture, texture);
            material.SetColor(ShaderProperty.Color, texture ? Color.white : Color.clear);
        }

        private void HandleSettingsChanged(object sender, EventArgs e)
        {
            SetBodyVisibilityAccordingToState();
            SetArmsVisibilityAccordingToState();
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

        private Texture2D GetArmsTexture()
        {
            return isShowingArms ? VrAssetLoader.ArmsCutoutTexture : null;
        }

        private void SetBodyVisibilityAccordingToState()
        {
            MakeMaterialTextureTransparent(bodyMaterial, GetBodyTexture());
            MakeMaterialTextureTransparent(backpackMaterial, GetBackpackTexture());
        }

        private void SetArmsVisibilityAccordingToState()
        {
            MakeMaterialTextureTransparent(armsMaterial, GetArmsTexture());
        }
    }
}