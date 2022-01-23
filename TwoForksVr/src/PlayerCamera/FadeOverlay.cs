using TwoForksVr.Stage;
using UnityEngine;
using UnityEngine.UI;

namespace TwoForksVr.PlayerCamera
{
    public class FadeOverlay: MonoBehaviour
    {
        public static FadeOverlay Instance; // TODO no singleton.
        private Canvas canvas;
        private Image image;
        private float targetAlpha;
        private float alphaLerpT;
        public const float Duration = 0.1f;

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
            Instance.image.color = new Color(0, 0, 0, 0);

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