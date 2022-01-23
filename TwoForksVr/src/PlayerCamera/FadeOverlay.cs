using TwoForksVr.Stage;
using UnityEngine;
using UnityEngine.UI;

namespace TwoForksVr.PlayerCamera
{
    public class FadeOverlay: MonoBehaviour
    {
        private Canvas canvas;
        private Image image;
        private float targetAlpha;
        private float alphaLerpT;
        public const float Duration = 0.1f;

        public static FadeOverlay Create(VrStage vrStage)
        {
            var gameObject = new GameObject("FadeOverlay");
            gameObject.transform.SetParent(vrStage.transform, false);
            var fadeOverlay = gameObject.AddComponent<FadeOverlay>();
            
            fadeOverlay.canvas = gameObject.AddComponent<Canvas>();
            fadeOverlay.canvas.enabled = false;
            fadeOverlay.canvas.renderMode = RenderMode.ScreenSpaceCamera;
            fadeOverlay.canvas.planeDistance = 1f;
            fadeOverlay.canvas.sortingOrder = 1000;
            
            fadeOverlay.image = gameObject.AddComponent<Image>();
            fadeOverlay.image.color = new Color(0, 0, 0, 0);

            return fadeOverlay;
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

        private void Update()
        {
            if (Mathf.Abs(targetAlpha - image.color.a) < 0.01f) return;
            alphaLerpT += Time.unscaledDeltaTime;
            image.color = new Color(0, 0, 0, Mathf.Lerp(image.color.a, targetAlpha, alphaLerpT/Duration));
        }

        public void FadeToBlack()
        {
            alphaLerpT = 0;
            targetAlpha = 1;
        }

        public void FadeToClear()
        {
            alphaLerpT = 0;
            targetAlpha = 0;
        }
    }
}