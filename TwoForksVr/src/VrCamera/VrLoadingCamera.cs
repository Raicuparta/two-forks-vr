using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.VrCamera
{
    public class VrLoadingCamera : MonoBehaviour
    {
        private vgLoadingCamera loadingCamera;
        private Camera vrCamera;

        public static void Create(vgLoadingCamera loadingCamera)
        {
            var gameObject = new GameObject("VrLoadingCamera");
            var instance = gameObject.AddComponent<VrLoadingCamera>();
            instance.loadingCamera = loadingCamera;
            instance.SetUpVrCamera();
            instance.SetUpOnlyLoadOnce();
            instance.SetUpParentCanvas();
            instance.DisableOriginalCamera();
        }

        private void LateUpdate()
        {
            if (!loadingCamera)
            {
                Logs.LogError("VrLoadingCamera missing vgLoadingCamera property");
                return;
            }

            vrCamera.enabled = loadingCamera.isActiveAndEnabled;
        }

        private void DisableOriginalCamera()
        {
            var camera = loadingCamera.GetComponent<Camera>();
            camera.enabled = false;
        }

        private void SetUpVrCamera()
        {
            vrCamera = gameObject.AddComponent<Camera>();
            vrCamera.cullingMask = LayerMask.GetMask("UI");
            vrCamera.clearFlags = CameraClearFlags.SolidColor;
            vrCamera.backgroundColor = Color.black;
        }

        private void SetUpOnlyLoadOnce()
        {
            var onlyLoadOnce = gameObject.AddComponent<vgOnlyLoadOnce>();
            onlyLoadOnce.dontDestroyOnLoad = true;
            onlyLoadOnce.dontDestroyOnReset = true;
        }

        private void SetUpParentCanvas()
        {
            Logs.LogInfo($"SetUpParentCanvas loadingCamera {loadingCamera}");
            Logs.LogInfo($"SetUpParentCanvas loadingCamera.transform {loadingCamera.transform}");
            Logs.LogInfo($"SetUpParentCanvas loadingCamera.transform.parent {loadingCamera.transform.parent}");
            Logs.LogInfo(
                $"SetUpParentCanvas loadingCamera.transform.parent.parent {loadingCamera.transform.parent.parent}");
            Logs.LogInfo(
                $"SetUpParentCanvas loadingCamera.transform.parent.parent.gameObject {loadingCamera.transform.parent.parent.gameObject}");
            var parentCanvas = loadingCamera.transform.parent.parent.gameObject.AddComponent<Canvas>();
            Logs.LogInfo($"SetUpParentCanvas parentCanvas {parentCanvas}");
            parentCanvas.worldCamera = vrCamera;
            parentCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        }
    }
}