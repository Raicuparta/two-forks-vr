using TwoForksVr.Assets;
using TwoForksVr.Stage;
using UnityEngine;
using UnityEngine.UI;

namespace TwoForksVr.VrCamera
{
    public class FadeOverlay : MonoBehaviour
    {
        public const float Duration = 0.1f;
        private float alphaLerpT;
        private Canvas canvas;
        private Image image;
        private float targetAlpha;

        public static FadeOverlay Create(VrStage vrStage)
        {
            var gameObject = Instantiate(VrAssetLoader.FadeOverlayPrefab, vrStage.transform, false);
            var fadeOverlay = gameObject.AddComponent<FadeOverlay>();

            fadeOverlay.canvas = gameObject.GetComponent<Canvas>();
            fadeOverlay.canvas.enabled = false;
            fadeOverlay.canvas.renderMode = RenderMode.ScreenSpaceCamera;

            fadeOverlay.image = gameObject.GetComponentInChildren<Image>();
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
            image.color = new Color(0, 0, 0, Mathf.Lerp(image.color.a, targetAlpha, alphaLerpT / Duration));
        }

        public void FadeToBlack()
        {
            image.color = new Color(0, 0, 0, 0);
            alphaLerpT = 0;
            targetAlpha = 1;
        }

        public void FadeToClear()
        {
            image.color = new Color(0, 0, 0, 1);
            alphaLerpT = 0;
            targetAlpha = 0;
        }
    }
}