using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using UnityEngine;
using UnityEngine.Rendering;

namespace TwoForksVr.PlayerBody
{
    public class VRBodyManager : MonoBehaviour
    {
        private vgCameraController cameraController;
        private Camera camera;
        private CharacterController characterController;
        private vgPlayerController playerController;
        private vgPlayerNavigationController navigationController;
        private Vector3 prevCameraPosition;

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
            instance.camera = camera;
            instance.cameraController = playerController.cameraController;
            instance.prevCameraPosition = camera.transform.position;
            instance.playerController = playerController;
            instance.navigationController = playerController.GetComponentInChildren<vgPlayerNavigationController>();
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
            UpdateRoomScalePosition();
        }

        private void UpdateRoomScalePosition()
        {
            var playerBody = transform.parent.parent;
            
            cameraController.transform.position = playerBody.position;

            var cameraTransform = camera.transform;
            var cameraPosition = cameraTransform.localPosition;
            
            var cameraMovement = cameraPosition - prevCameraPosition;
            cameraMovement.y = 0;
            
            var worldCameraMovement = VRStage.Instance.transform.TransformVector(cameraMovement);

            var magnitude = worldCameraMovement.magnitude;
            if (magnitude < 0.005f) return;
            prevCameraPosition = cameraPosition;
            
            if (magnitude > 1f || !navigationController.onGround) return;

            
            var groundMovement = Vector3.ProjectOnPlane(worldCameraMovement, navigationController.groundNormal);

            characterController.Move(groundMovement);
            
            // This probably breaks stuff elsewhere.
            navigationController.positionLastFrame = playerBody.position;

            VRStage.Instance.transform.position -= groundMovement;
        }

        private void HideBody()
        {
            var renderer = transform.GetComponent<SkinnedMeshRenderer>();
            renderer.shadowCastingMode = ShadowCastingMode.TwoSided;

            var materials = renderer.materials;

            var bodyMaterial = materials[0];
            MakeMaterialTextureTransparent(bodyMaterial, VRAssetLoader.BodyCutoutTexture);

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
