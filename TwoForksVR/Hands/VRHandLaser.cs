using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Valve.VR;

namespace TwoForksVR
{
    class VRHandLaser: MonoBehaviour
    {
        public static VRHandLaser Instance;
        private static LineRenderer lineRenderer;

        private void Start()
        {
            Instance = this;

            name = "VR Laser";

            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.SetPositions(new[] { Vector3.zero, Vector3.forward });
            lineRenderer.startWidth = 0.005f;
            lineRenderer.endWidth = 0.001f;
            lineRenderer.endColor = new Color(1, 1, 1, 0.3f);
            lineRenderer.startColor = Color.clear;
            lineRenderer.material.shader = Shader.Find("Particles/Alpha Blended Premultiply");
            lineRenderer.material.SetColor("_Color", new Color(0.8f, 0.8f, 0.8f));
            lineRenderer.enabled = false;
        }

        private bool ShouldLaserBeVisible()
        {
            return vgHudManager.Instance?.currentTarget != null || SteamVR_Actions.default_Interact.state;
        }

        private void UpdateLaserVisibility()
        {
            lineRenderer.enabled = ShouldLaserBeVisible();
        }

        private void Update()
        {
            UpdateLaserVisibility();
        }
    }
}
