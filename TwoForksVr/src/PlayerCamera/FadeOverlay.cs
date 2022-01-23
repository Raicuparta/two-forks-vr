using TwoForksVr.Stage;
using UnityEngine;
using UnityEngine.UI;

namespace TwoForksVr.PlayerCamera
{
    public class FadeOverlay: MonoBehaviour
    {
        private Canvas canvas;
        private Image image;
        public static FadeOverlay Instance; // TODO no singleton.
        
        public static FadeOverlay Create(VrStage vrStage)
        {
            var gameObject = new GameObject("FadeOverlay");
            gameObject.transform.SetParent(vrStage.transform, false);
            Instance = gameObject.AddComponent<FadeOverlay>();
            
            Instance.canvas = gameObject.AddComponent<Canvas>();
            Instance.canvas.enabled = false;
            Instance.canvas.renderMode = RenderMode.ScreenSpaceCamera;
            Instance.canvas.planeDistance = 1f;
            Instance.canvas.sortingOrder = 1000;
            
            Instance.image = gameObject.AddComponent<Image>();
            Instance.image.color = Color.clear;

            return Instance;
        }

        public void SetUp(Camera camera)
        {
            if (camera)
            {
                canvas.worldCamera = camera;
                canvas.enabled = true;
            }
            else
            {
                canvas.enabled = false;
            }
        }

        public void FadeToBlack()
        {
            image.color = Color.black;
        }

        public void FadeToClear()
        {
            image.color = Color.clear;
        }
    }
}