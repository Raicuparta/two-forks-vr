using TwoForksVr.Assets;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.VrCamera
{
    // TODO clean this up. This was adapted from SteamVR code and it has some unnecessary stuff.
    public class FadeOverlay : MonoBehaviour
    {
        public const float Duration = 0.1f;
        private static Material fadeMaterial;
        private static int fadeMaterialColorID = -1;

        private Color currentColor = new Color(0, 0, 0, 0); // default starting color: black and fully transparent

        // the delta-color is basically the "speed / second" at which the current color should change
        private Color deltaColor = new Color(0, 0, 0, 0);

        private Color targetColor = new Color(0, 0, 0, 0); // default target color: black and fully transparent

        public static void Create(Camera camera)
        {
            camera.gameObject.AddComponent<FadeOverlay>();
        }

        private void OnEnable()
        {
            if (fadeMaterial == null)
            {
                fadeMaterial = new Material(VrAssetLoader.FadeShader);
                fadeMaterialColorID = Shader.PropertyToID("fadeColor");
            }

            SteamVR_Events.Fade.Listen(OnStartFade);
            SteamVR_Events.FadeReady.Send();
        }

        private void OnDisable()
        {
            SteamVR_Events.Fade.Remove(OnStartFade);
        }

        private void OnPostRender()
        {
            if (currentColor != targetColor)
            {
                // if the difference between the current alpha and the desired alpha is smaller than delta-alpha * deltaTime, then we're pretty much done fading:
                if (Mathf.Abs(currentColor.a - targetColor.a) < Mathf.Abs(deltaColor.a) * Time.deltaTime)
                {
                    currentColor = targetColor;
                    deltaColor = new Color(0, 0, 0, 0);
                }
                else
                {
                    currentColor += deltaColor * Time.deltaTime;
                }
            }

            if (currentColor.a > 0 && fadeMaterial)
            {
                fadeMaterial.SetColor(fadeMaterialColorID, currentColor);
                fadeMaterial.SetPass(0);
                GL.Begin(GL.QUADS);

                GL.Vertex3(-1, -1, 0);
                GL.Vertex3(1, -1, 0);
                GL.Vertex3(1, 1, 0);
                GL.Vertex3(-1, 1, 0);
                GL.End();
            }
        }

        public static void StartFade(Color newColor, float duration, bool fadeOverlay = false)
        {
            SteamVR_Events.Fade.Send(newColor, duration, fadeOverlay);
        }

        public void OnStartFade(Color newColor, float duration, bool fadeOverlay)
        {
            if (duration > 0.0f)
            {
                targetColor = newColor;
                deltaColor = (targetColor - currentColor) / duration;
            }
            else
            {
                currentColor = newColor;
            }
        }
    }
}