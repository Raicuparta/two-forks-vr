using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using UnityEngine;
using UnityEngine.Rendering;
using Valve.VR;

namespace TwoForksVr.PlayerBody
{
    public class VRBodyManager : MonoBehaviour
    {
        private vgCameraController cameraController;
        private Camera camera;
        private LateUpdateFollow cameraFollow;
        private CharacterController characterController;
        private vgPlayerController playerController;

        public static void Create(vgPlayerController playerController)
        {
            var playerTransform = playerController.transform;
            var playerBody = playerTransform.Find("henry/body").gameObject;
            playerBody.layer = LayerMask.NameToLayer("UI");
            var existingBodyManager = playerBody.GetComponent<VRBodyManager>();
            if (existingBodyManager) return;

            var camera = playerController.cameraController.GetComponentInChildren<Camera>();

            VRStage.Instance.SetUp(
                camera,
                playerTransform
            );

            var instance = playerBody.AddComponent<VRBodyManager>();
            instance.cameraFollow = playerController.cameraController.GetComponentInChildren<LateUpdateFollow>();
            instance.camera = camera;
            instance.cameraController = playerController.cameraController;
            instance.prevCameraPosition = camera.transform.position;
            instance.playerController = playerController;
        }
        
        private void Start()
        {
            HideBody();
        }

        private void Update()
        {
            if (characterController == null)
            {
                characterController = playerController.characterController;
            }
            UpdateCameraPosition();
        }

        private Vector3 prevCameraPosition;
        private void UpdateCameraPosition()
        {
            if (SteamVR_Actions.default_Recenter.state)
            {
                return;
            }

            if (characterController == null)
            {
                Logs.LogInfo("No character controller");
                return;
            }
            
            var playerBody = transform.parent.parent;
            cameraController.transform.position = playerBody.position;
            
            var cameraTransform = camera.transform;
            var cameraPosition = cameraTransform.localPosition;
            
            var cameraMovement = cameraPosition - prevCameraPosition;
            cameraMovement.y = 0;

            var worldCameraMovement = cameraTransform.TransformVector(cameraMovement);
            
            if (worldCameraMovement.magnitude <= 0.005f) return;
            
            // playerBody.position += worldCameraMovement;
            characterController.Move(worldCameraMovement);
            cameraController.transform.position -= worldCameraMovement;
            prevCameraPosition = cameraPosition;
            Logs.LogInfo(worldCameraMovement.x);
        }

        private void HideBody()
        {
            var renderer = transform.GetComponent<SkinnedMeshRenderer>();
            renderer.shadowCastingMode = ShadowCastingMode.TwoSided;

            var materials = renderer.materials;

            var bodyMaterial = materials[0];
            MakeMaterialTextureTransparent(bodyMaterial);

            var backpackMaterial = materials[1];
            MakeMaterialTextureTransparent(backpackMaterial);

            var armsMaterial = materials[2];
            MakeMaterialTextureTransparent(armsMaterial, VRAssetLoader.ArmsCutoutTexture);
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
