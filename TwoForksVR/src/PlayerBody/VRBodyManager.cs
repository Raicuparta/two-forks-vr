using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using UnityEngine;
using UnityEngine.Rendering;

namespace TwoForksVr.PlayerBody
{
    public class VRBodyManager : MonoBehaviour
    {
        private Camera camera;
        private CharacterController characterController;
        private vgPlayerController playerController;
        private vgPlayerNavigationController navigationController;
        private Vector3 prevCameraPosition;
        private Vector3 prevForward;

        public static void Create(vgPlayerController playerController)
        {
            var playerTransform = playerController.transform;
            var playerBody = playerTransform.Find("henry/body").gameObject;
            LayerHelper.SetLayer(playerBody, GameLayer.PlayerBody);
            var existingBodyManager = playerBody.GetComponent<VRBodyManager>();
            if (existingBodyManager) return;

            var camera = playerController.cameraController.GetComponentInChildren<Camera>();

            VRStage.Instance.SetUp(
                camera,
                playerTransform
            );

            var instance = playerBody.AddComponent<VRBodyManager>();
            instance.camera = camera;
            instance.prevCameraPosition = camera.transform.position;
            instance.playerController = playerController;
            instance.navigationController = playerController.GetComponentInChildren<vgPlayerNavigationController>();
        }
        
        private void Start()
        {
            prevForward = GetCameraForward();
            HideBodyParts();
        }

        private void Update()
        {
            if (characterController == null)
            {
                characterController = playerController.characterController;
            }
            
            UpdateRoomScalePosition();
            UpdateRotation();
        }

        private Vector3 GetCameraForward()
        {
            return camera.transform.parent.InverseTransformDirection(
                Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up));
        }

        private void UpdateRoomScalePosition()
        {
            var playerBody = transform.parent.parent;
            
            var cameraTransform = camera.transform;
            var cameraPosition = cameraTransform.localPosition;
            
            var localPositionDelta = cameraPosition - prevCameraPosition;
            localPositionDelta.y = 0;

            var worldPositionDelta = VRStage.Instance.transform.TransformVector(localPositionDelta);

            prevCameraPosition = cameraPosition;
            
            if (worldPositionDelta.sqrMagnitude > 1f || !navigationController.onGround || !navigationController.enabled) return;
            
            var groundedPositionDelta = Vector3.ProjectOnPlane(worldPositionDelta, navigationController.groundNormal);

            characterController.Move(groundedPositionDelta);
            
            // This probably breaks stuff elsewhere.
            navigationController.positionLastFrame = playerBody.position;

            VRStage.Instance.transform.position -= groundedPositionDelta;
        }

        private void UpdateRotation()
        {
            if (!navigationController.onGround || !navigationController.enabled) return;

            var cameraForward = GetCameraForward();
            var angleDelta = MathHelper.SignedAngle(prevForward, cameraForward, Vector3.up);
            prevForward = cameraForward;
            characterController.transform.Rotate(Vector3.up, angleDelta);
            
            VRStage.Instance.Recenter();
        }

        // Hides body parts by either making them completely invisible,
        // or by using transparent textures to leave parts visible (hands and feet).
        private void HideBodyParts()
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
