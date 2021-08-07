using TwoForksVR.Assets;
using UnityEngine;
using TwoForksVR.Stage;
using Harmony;
using MelonLoader;

namespace TwoForksVR.PlayerBody
{
    public class VRBodyManager : MonoBehaviour
    {
        public static VRBodyManager Create(Transform playerTransform)
        {
            var playerBody = playerTransform.Find("henry/body").gameObject;
            playerBody.layer = LayerMask.NameToLayer("UI");
            var existingBodyManager = playerBody.GetComponent<VRBodyManager>();
            if (existingBodyManager) return existingBodyManager;

            var camera = playerTransform
                .parent
                .GetComponentInChildren<vgCameraController>()
                .GetComponentInChildren<Camera>();

            VRStage.Instance.SetUp(
                camera: camera,
                playerTransform: playerTransform
            );
            return playerBody.AddComponent<VRBodyManager>();
        }

        private void Start()
        {
            HideBody();
        }

        private void HideBody()
        {
            var renderer = transform.GetComponent<SkinnedMeshRenderer>();
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
    }

    [HarmonyPatch(typeof(vgPlayerController), "Awake")]
    public class CreateBodyManager
    {
        public static void Prefix(vgPlayerController __instance)
        {
            VRBodyManager.Create(__instance.transform);
        }
    }
}
