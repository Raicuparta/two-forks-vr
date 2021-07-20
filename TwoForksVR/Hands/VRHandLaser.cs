using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Valve.VR;

namespace TwoForksVR.Hands
{
    class VRHandLaser: MonoBehaviour
    {
        private LineRenderer lineRenderer;

        public static VRHandLaser Create(Transform parent)
        {
            var instance = new GameObject("VRHandLaser").AddComponent<VRHandLaser>();
            instance.transform.SetParent(parent);
            return instance;
        }

        private void Start()
        {
            UseHandLaserForTargeting.LaserTransform = transform;

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

        private void UpdateLaserParent()
        {
            if (SteamVR_Actions.default_Interact.GetStateDown(SteamVR_Input_Sources.LeftHand))
            {
                transform.SetParent(VRHandsManager.Instance.LeftHand, false);
            }
            if (SteamVR_Actions.default_Interact.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                transform.SetParent(VRHandsManager.Instance.RightHand, false);
            }
        }

        private void UpdateLaserVisibility()
        {
            lineRenderer.enabled = vgHudManager.Instance?.currentTarget != null || SteamVR_Actions.default_Interact.state;
        }

        private void Update()
        {
            UpdateLaserParent();
            UpdateLaserVisibility();
        }
    }
}
