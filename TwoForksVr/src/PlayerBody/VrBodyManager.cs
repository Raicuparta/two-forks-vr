using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using UnityEngine;
using UnityEngine.Rendering;

namespace TwoForksVr.PlayerBody
{
    public class VrBodyManager : MonoBehaviour
    {
        public static void Create(vgPlayerController playerController)
        {
            var playerTransform = playerController.transform;
            var playerBody = playerTransform.Find("henry/body").gameObject;
            LayerHelper.SetLayer(playerBody, GameLayer.PlayerBody);
            var existingBodyManager = playerBody.GetComponent<VrBodyManager>();
            if (existingBodyManager) return;

            var camera = playerController.cameraController.GetComponentInChildren<Camera>();

            VrStage.Instance.SetUp(
                camera,
                playerTransform
            );

            playerBody.AddComponent<VrBodyManager>();
        }
        
        private void Start()
        {
            HideBodyParts();
        }

        // Hides body parts by either making them completely invisible,
        // or by using transparent textures to leave parts visible (hands and feet).
        private void HideBodyParts()
        {
            var renderer = transform.GetComponent<SkinnedMeshRenderer>();
            renderer.shadowCastingMode = ShadowCastingMode.TwoSided;

            var materials = renderer.materials;
            
            var bodyMaterial = materials[0];
            MakeMaterialTextureTransparent(bodyMaterial, VrAssetLoader.BodyCutoutTexture);
            
            var backpackMaterial = materials[1];
            MakeMaterialTextureTransparent(backpackMaterial);

            var armsMaterial = materials[2];
            MakeMaterialTextureTransparent(armsMaterial, VrAssetLoader.ArmsCutoutTexture);
        }

        private static void MakeMaterialTextureTransparent(Material material, Texture2D texture = null)
        {
            var cutoutShader = Shader.Find("Marmoset/Transparent/Cutout/Bumped Specular IBL");
            material.shader = cutoutShader;
            material.SetTexture(ShaderProperty.MainTexture, texture);
            if (!texture) material.SetColor(ShaderProperty.Color, Color.clear);
        }
    }
}
