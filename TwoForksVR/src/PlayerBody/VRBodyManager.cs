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
        private Transform debugCube;
        private vgPlayerNavigationController navigationController;


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
            instance.navigationController = playerController.GetComponentInChildren<vgPlayerNavigationController>();

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = Vector3.one * 0.5f;
            cube.GetComponent<Collider>().enabled = false;
            var cubeParent = new GameObject("DebugCube");
            cube.transform.SetParent(cubeParent.transform, false);
            cube.transform.localPosition = Vector3.forward * 1f;
            instance.debugCube = cubeParent.transform;
        }
        
        private void Start()
        {
            HideBody();
        }

        private void LateUpdate()
        {
            if (characterController == null)
            {
                characterController = playerController.characterController;
            }
            UpdateCameraPosition();
        }

        private Vector3 prevCameraPosition;
        private Vector3 prevPlayerPosition;
        private void UpdateCameraPosition()
        {
            var playerBody = transform.parent.parent;

            var playerMovement = playerController.transform.position - prevPlayerPosition;
            if (true)
            {
                // return;
            }
            else
            {
                debugCube.position = playerBody.position;
                debugCube.rotation = playerBody.rotation;
            }
            cameraController.transform.position = playerBody.position;

            // if (characterController == null)
            // {
            //     Logs.LogInfo("No character controller");
            //     return;
            // }
            //
            //
            var cameraTransform = camera.transform;
            var cameraPosition = cameraTransform.localPosition;
            
            var cameraMovement = cameraPosition - prevCameraPosition;
            cameraMovement.y = 0;
            
            var worldCameraMovement = VRStage.Instance.transform.TransformVector(cameraMovement);

            var magnitude = worldCameraMovement.magnitude;
            if (magnitude < 0.005f) return;
            prevPlayerPosition = playerController.transform.position;
            prevCameraPosition = cameraPosition;
            if (magnitude > 1f) return;
            // navigationController.positionLastFrame = playerBody.position;
            debugCube.position += worldCameraMovement;
            playerBody.position += worldCameraMovement;
            //
            //
            // characterController.Move(worldCameraMovement);
            // cameraFollow.LocalPosition -= worldCameraMovement;
            VRStage.Instance.transform.position -= worldCameraMovement;
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
