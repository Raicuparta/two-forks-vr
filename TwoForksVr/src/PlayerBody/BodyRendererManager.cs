using System;
using TwoForksVr.Assets;
using TwoForksVr.Limbs;
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
        private VrLimbManager limbManager;
        private vgPlayerNavigationController navigationController;
        private SkinnedMeshRenderer playerRenderer;
        private TeleportController teleportController;
        private float timeToShowArms;

        public static BodyRendererManager Create(VrStage stage, TeleportController teleportController,
            VrLimbManager limbManager)
        {
            var instance = stage.gameObject.AddComponent<BodyRendererManager>();
            instance.teleportController = teleportController;
            instance.limbManager = limbManager;
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
            limbManager.StopTrackingOriginalHands();
        }

        private void UpdateIsShowingArms()
        {
            var shouldShowArms = ShouldShowArms();
            if (!isCountingTimeToShowArms && shouldShowArms)
            {
                isCountingTimeToShowArms = true;
            }
            else if (isCountingTimeToShowArms && !shouldShowArms)
            {
                timeToShowArms = 0;
                isCountingTimeToShowArms = false;
                SetArmsTexture();
                limbManager.StartTrackingOriginalHands();
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
            return !teleportController.IsTeleporting() && !IsNavigationControllerEnabled();
        }

        private void SetUpMaterials()
        {
            playerRenderer.shadowCastingMode = ShadowCastingMode.TwoSided;

            playerRenderer.materials = new[]
            {
                VrAssetLoader.HenryBodyMaterial,
                VrAssetLoader.HenryBackpackMaterial,
                VrAssetLoader.HenryArmsMaterial
            };

            bodyMaterial = playerRenderer.materials[0];
            armsMaterial = playerRenderer.materials[2];

            SetBodyTexture();
            SetArmsTexture();
        }

        private void SetTexture(Material material, Color color)
        {
            if (!material) return;
            material.color = color;
        }

        private void HandleSettingsChanged(object sender, EventArgs e)
        {
            SetBodyTexture();
            SetArmsTexture();
        }

        private Color GetBodyTexture()
        {
            if (isShowingFullBody) return Color.white;
            return VrSettings.ShowLegs.Value ? Color.white : Color.clear;
        }

        private Color GetArmsTexture()
        {
            return isCountingTimeToShowArms ? Color.white : Color.clear;
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