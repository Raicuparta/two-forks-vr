using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Raicuparta.TwoForksVR
{
    class VRHandLaser: MonoBehaviour
    {
        private static Transform selfTransform;

        private void Start()
        {
            selfTransform = transform;

            name = "VR Laser";

            var lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.SetPositions(new[] { Vector3.zero, Vector3.forward });
            lineRenderer.startWidth = 0.005f;
            lineRenderer.endWidth = 0.001f;
            lineRenderer.endColor = new Color(1, 1, 1, 0.3f);
            lineRenderer.startColor = Color.clear;
            lineRenderer.material.shader = Shader.Find("Particles/Alpha Blended Premultiply");
            lineRenderer.material.SetColor("_Color", new Color(0.8f, 0.8f, 0.8f));
        }

        [HarmonyPatch(typeof(vgPlayerTargeting), "UpdateTarget")]
        public class PatchUpdateTarget
        {
            public static void Prefix(ref Vector3 cameraFacing, ref Vector3 cameraOrigin)
            {
                cameraFacing = selfTransform.forward;
                cameraOrigin = selfTransform.position;
            }
        }
    }
}
