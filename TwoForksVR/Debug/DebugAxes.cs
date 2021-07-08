using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TwoForksVR
{
    class DebugAxes: MonoBehaviour
    {
        private void Start()
        {
            CreateLine(Color.red, Vector3.right);
            CreateLine(Color.green, Vector3.forward);
            CreateLine(Color.blue, Vector3.up);
        }

        private void CreateLine(Color color, Vector3 destination)
        {
            var line = new GameObject("debugLine").transform;
            line.transform.SetParent(transform, false);

            var lineRenderer = line.gameObject.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.SetPositions(new[] { Vector3.zero, destination });
            lineRenderer.startWidth = 0.005f;
            lineRenderer.endWidth = 0.005f;
            lineRenderer.endColor = color;
            lineRenderer.startColor = color;
            lineRenderer.material.shader = Shader.Find("Particles/Alpha Blended Premultiply");
            lineRenderer.material.SetColor("_Color", color);
        }
    }
}
