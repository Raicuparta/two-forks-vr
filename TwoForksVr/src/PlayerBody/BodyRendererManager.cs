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
        // After vgPlayerNavigationController has been disabled for this time in seconds, the hands become visible.
        private const float minimumNavigationDisabledTimeToShowArms = 0.3f;
        private Material armsMaterial;
        private Material backpackMaterial;
        private Texture backpackTexture;
        private Material bodyMaterial;
        private Texture bodyTexture;
        private Shader cutoutShader;
        private bool isCountingTimeToShowArms;
        private bool isShowingFullBody;
        private vgPlayerNavigationController navigationController;
        private SkinnedMeshRenderer playerRenderer;
        private TeleportController teleportController;

        private float timeToShowArms;
        // private Shader transparentShader;

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
            playerRenderer = playerBody.GetComponent<SkinnedMeshRenderer>();
            navigationController = playerController.navController;

            SetUpMaterials();
        }

        private void Awake()
        {
            VrSettings.Config.SettingChanged += HandleSettingsChanged;
            // transparentShader = Shader.Find("Valve/VR/Highlight");
            cutoutShader = Shader.Find("Marmoset/Transparent/Cutout/Bumped Specular IBL");
        }

        private void Update()
        {
            UpdateShowFullBody();
            UpdateIsShowingArms();
            UpdateArmsVisibility();
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

        private void UpdateArmsVisibility()
        {
            if (isCountingTimeToShowArms) timeToShowArms += Time.deltaTime;

            if (timeToShowArms <= minimumNavigationDisabledTimeToShowArms) return;

            timeToShowArms = 0;
            SetArmsVisibilityAccordingToState();
        }

        private void UpdateIsShowingArms()
        {
            var shoudlShowArms = ShouldShowArms();
            if (!isCountingTimeToShowArms && shoudlShowArms)
            {
                isCountingTimeToShowArms = true;
            }
            else if (isCountingTimeToShowArms && !shoudlShowArms)
            {
                timeToShowArms = 0;
                isCountingTimeToShowArms = false;
                SetArmsVisibilityAccordingToState();
            }
        }

        private bool ShouldShowFullBody()
        {
            return teleportController.IsTeleporting();
        }

        private bool IsNavigationControllerEnabled()
        {
            return navigationController && navigationController.enabled;
        }

        private bool ShouldShowArms()
        {
            return VrSettings.UseOriginalHandsWhileNavigationDisabled.Value && !teleportController.IsTeleporting() &&
                   !IsNavigationControllerEnabled();
        }

        private void SetUpMaterials()
        {
            playerRenderer.shadowCastingMode = ShadowCastingMode.TwoSided;

            var materials = playerRenderer.materials;

            bodyMaterial = materials[0];
            bodyTexture = bodyMaterial.mainTexture;

            backpackMaterial = materials[1];
            backpackTexture = backpackMaterial.mainTexture;
            MakeMaterialTextureTransparent(backpackMaterial);

            armsMaterial = materials[2];

            SetBodyVisibilityAccordingToState();
            SetArmsVisibilityAccordingToState();
        }

        private void MakeMaterialTextureTransparent(Material material, Texture texture = null)
        {
            if (!material) return;
            material.shader = texture ? VrAssetLoader.HighlightShader : cutoutShader;
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
            if (isShowingFullBody) return VrAssetLoader.BodyCutoutTexture;
            return VrSettings.ShowFeet.Value ? VrAssetLoader.BodyCutoutTexture : null;
        }

        private Texture2D GetArmsTexture()
        {
            return isCountingTimeToShowArms ? VrAssetLoader.ArmsCutoutTexture : null;
        }

        private void SetBodyVisibilityAccordingToState()
        {
            MakeMaterialTextureTransparent(bodyMaterial, GetBodyTexture());
        }

        private void SetArmsVisibilityAccordingToState()
        {
            MakeMaterialTextureTransparent(armsMaterial, GetArmsTexture());
        }
    }
}