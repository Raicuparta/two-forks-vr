using UnityEngine;

namespace TwoForksVR.Helpers
{
    public static class MathHelper
    {
        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            var unsignedAngle = Vector3.Angle(from, to);

            var crossX = from.y * to.z - from.z * to.y;
            var crossY = from.z * to.x - from.x * to.z;
            var crossZ = from.x * to.y - from.y * to.x;
            var sign = Mathf.Sign(axis.x * crossX + axis.y * crossY + axis.z * crossZ);
            return unsignedAngle * sign;
        }

        public static Vector3 PositionAroundCircle(int index, int totalCount, float circleRadius)
        {
            var angle = index * Mathf.PI * 2f / totalCount;
            return new Vector3(Mathf.Cos(angle) * circleRadius, Mathf.Sin(angle) * circleRadius, 0);
        }

        public static float GetSquareDistance(Vector3 pointA, Vector3 pointB)
        {
            return (pointA - pointB).sqrMagnitude;
        }

        public static float GetSquareDistance(Transform transformA, Transform transformB)
        {
            return GetSquareDistance(transformA.position, transformB.position);
        }
    }
}