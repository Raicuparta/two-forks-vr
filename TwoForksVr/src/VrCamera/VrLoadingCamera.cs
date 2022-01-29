using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.VrCamera
{
    public class VrLoadingCamera : MonoBehaviour
    {
        private vgLoadingCamera loadingCamera;
        private Camera vrCamera;

        public static void Create(vgLoadingCamera loadingCamera, Camera vrCamera)
        {
            var instance = vrCamera.gameObject.AddComponent<VrLoadingCamera>();
            instance.loadingCamera = loadingCamera;
            var onlyLoadOnce = instance.gameObject.AddComponent<vgOnlyLoadOnce>();
            onlyLoadOnce.dontDestroyOnLoad = true;
            onlyLoadOnce.dontDestroyOnReset = true;
            instance.vrCamera = vrCamera;
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
    }
}