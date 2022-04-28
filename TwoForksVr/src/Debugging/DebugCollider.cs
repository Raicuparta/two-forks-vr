using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.Debugging;

internal class DebugCollider : MonoBehaviour
{
    private void Start()
    {
        var colliders = GetComponentsInParent<BoxCollider>();

        foreach (var collider in colliders)
        {
            var size = collider.size / 2;
            var center = collider.center;

            var scale = transform.localScale.x;
            var position = transform.position;

            var boundPoint1 = center + size;
            var boundPoint2 = center - size;
            var boundPoint3 = new Vector3(boundPoint1.x, boundPoint1.y, boundPoint2.z);
            var boundPoint4 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint1.z);
            var boundPoint5 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint1.z);
            var boundPoint6 = new Vector3(boundPoint1.x, boundPoint2.y, boundPoint2.z);
            var boundPoint7 = new Vector3(boundPoint2.x, boundPoint1.y, boundPoint2.z);
            var boundPoint8 = new Vector3(boundPoint2.x, boundPoint2.y, boundPoint1.z);


            // rectangular cuboid
            // top of rectangular cuboid (6-2-8-4)
            CreateLine(boundPoint6, boundPoint2, Color.green);
            CreateLine(boundPoint2, boundPoint8, Color.green);
            CreateLine(boundPoint8, boundPoint4, Color.green);
            CreateLine(boundPoint4, boundPoint6, Color.green);

            // bottom of rectangular cuboid (3-7-5-1)
            CreateLine(boundPoint3, boundPoint7, Color.red);
            CreateLine(boundPoint7, boundPoint5, Color.red);
            CreateLine(boundPoint5, boundPoint1, Color.red);
            CreateLine(boundPoint1, boundPoint3, Color.red);

            // legs (6-3, 2-7, 8-5, 4-1)
            CreateLine(boundPoint6, boundPoint3, Color.blue);
            CreateLine(boundPoint2, boundPoint7, Color.blue);
            CreateLine(boundPoint8, boundPoint5, Color.blue);
            CreateLine(boundPoint4, boundPoint1, Color.blue);
        }
    }

    private void CreateLine(Vector3 origin, Vector3 destination, Color color)
    {
        var line = new GameObject("VrDebugLine").transform;
        line.transform.SetParent(transform, false);

        var lineRenderer = line.gameObject.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.SetPositions(new[] {origin, destination});
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.005f;
        lineRenderer.endColor = color;
        lineRenderer.startColor = color;
        lineRenderer.material.shader = Shader.Find("Particles/Alpha Blended Premultiply");
        lineRenderer.material.SetColor(ShaderProperty.Color, color);
    }
}