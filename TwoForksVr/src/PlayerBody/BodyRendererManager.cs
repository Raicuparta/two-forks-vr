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
        private Material bodyMaterial;
        private Shader cutoutShader;
        private bool isCountingTimeToShowArms;
        private bool isShowingFullBody;
        private vgPlayerNavigationController navigationController;
        private SkinnedMeshRenderer playerRenderer;
        private TeleportController teleportController;
        private float timeToShowArms;

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
            SetBodyTexture();
        }

        private void UpdateArmsVisibility()
        {
            if (isCountingTimeToShowArms) timeToShowArms += Time.deltaTime;

            if (timeToShowArms <= minimumNavigationDisabledTimeToShowArms) return;

            timeToShowArms = 0;
            SetArmsTexture();
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
                SetArmsTexture();
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

            var backpackMaterial = materials[1];
            SetTexture(backpackMaterial);

            armsMaterial = materials[2];

            SetBodyTexture();
            SetArmsTexture();
        }

        private void SetTexture(Material material, Texture texture = null)
        {
            if (!material) return;
            material.shader = texture ? VrAssetLoader.HighlightShader : cutoutShader;
            material.SetTexture(ShaderProperty.MainTexture, texture);
            material.SetColor(ShaderProperty.Color, texture ? Color.white : Color.clear);
        }

        private void HandleSettingsChanged(object sender, EventArgs e)
        {
            SetBodyTexture();
            SetArmsTexture();
        }

        private Texture2D GetBodyTexture()
        {
            if (isShowingFullBody) return VrAssetLoader.PlayerBodyTexture;
            return VrSettings.ShowLegs.Value ? VrAssetLoader.PlayerBodyTexture : null;
        }

        private Texture2D GetArmsTexture()
        {
            return isCountingTimeToShowArms ? VrAssetLoader.PlayerArmsTexture : null;
        }

        private void SetBodyTexture()
        {
            SetTexture(bodyMaterial, GetBodyTexture());
        }

        private void SetArmsTexture()
        {
            SetTexture(armsMaterial, GetArmsTexture());
        }
    }
}