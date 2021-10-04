using TwoForksVR.Hands.Patches;
using TwoForksVR.Helpers;
using UnityEngine;
using Valve.VR;

namespace TwoForksVR.Hands
{
    internal class VRHandLaser : MonoBehaviour
    {
        private Transform leftHand;
        private LineRenderer lineRenderer;
        private Transform rightHand;

        private void Start()
        {
            UseHandLaserForTargeting.LaserTransform = transform;

            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.SetPositions(new[] {Vector3.zero, Vector3.forward});
            lineRenderer.startWidth = 0.005f;
            lineRenderer.endWidth = 0.001f;
            lineRenderer.endColor = new Color(1, 1, 1, 0.3f);
            lineRenderer.startColor = Color.clear;
            lineRenderer.material.shader = Shader.Find("Particles/Alpha Blended Premultiply");
            lineRenderer.material.SetColor(ShaderProperty.Color, new Color(0.8f, 0.8f, 0.8f));
            lineRenderer.enabled = false;
        }

        private void Update()
        {
            UpdateLaserParent();
            UpdateLaserVisibility();
        }

        public static void Create(Transform leftHand, Transform rightHand)
        {
            var instance = new GameObject("VRHandLaser").AddComponent<VRHandLaser>();
            var instanceTransform = instance.transform;
            instanceTransform.SetParent(rightHand, false);
            instanceTransform.localEulerAngles = new Vector3(39.132f, 356.9302f, 0.3666f);
            instance.rightHand = rightHand;
            instance.leftHand = leftHand;
        }

        private void UpdateLaserParent()
        {
            if (SteamVR_Actions.default_Interact.GetStateDown(SteamVR_Input_Sources.LeftHand))
                transform.SetParent(leftHand, false);
            if (SteamVR_Actions.default_Interact.GetStateDown(SteamVR_Input_Sources.RightHand))
                transform.SetParent(rightHand, false);
        }

        private static bool HasCurrentTarget()
        {
            if (!vgHudManager.Instance) return false;
            return vgHudManager.Instance.currentTarget != null;
        }

        private void UpdateLaserVisibility()
        {
            if (!vgHudManager.Instance) return;
            lineRenderer.enabled =
                HasCurrentTarget() || SteamVR_Actions.default_Interact.state;
        }
    }
}