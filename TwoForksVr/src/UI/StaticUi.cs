using UnityEngine;

namespace TwoForksVr.UI
{
    public class StaticUi : MonoBehaviour
    {
        private static Camera camera;
        private Canvas parentCanvas;

        private void Awake()
        {
            parentCanvas = transform.parent.GetComponent<Canvas>();
            if (parentCanvas) return;
            parentCanvas = transform.parent.gameObject.AddComponent<Canvas>();
            parentCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            parentCanvas.planeDistance = 1;
            transform.localScale = Vector3.one * 0.5f;
        }

        private void Update()
        {
            transform.localPosition = Vector3.zero;
            if (parentCanvas) parentCanvas.worldCamera = camera;
        }

        public static void SetTargetCamera(Camera targetCamera)
        {
            camera = targetCamera;
        }
    }
}