using UnityEngine;

namespace TwoForksVr.Helpers
{
    public static class MathHelper
    {
        private const float cos45 = 0.70710678f;

        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis, float unsignedAngle)
        {
            var crossX = from.y * to.z - from.z * to.y;
            var crossY = from.z * to.x - from.x * to.z;
            var crossZ = from.x * to.y - from.y * to.x;
            var sign = Mathf.Sign(axis.x * crossX + axis.y * crossY + axis.z * crossZ);
            return unsignedAngle * sign;
        }
        
        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            var unsignedAngle = Vector3.Angle(from, to);
            return SignedAngle(from, to, axis, unsignedAngle);
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

        public static Vector2 ConvertCircleVectorToSquare(Vector2 input)
        {
            if (input.sqrMagnitude == 0) return Vector2.zero;

            var normal = input.normalized;
            float vectorX;
            float vectorY;

            if (normal.x != 0 && normal.y >= -cos45 && normal.y <= cos45)
                vectorX = normal.x >= 0 ? input.x / normal.x : -input.x / normal.x;
            else
                vectorX = input.x / Mathf.Abs(normal.y);

            if (normal.y != 0 && normal.x >= -cos45 && normal.x <= cos45)
                vectorY = normal.y >= 0 ? input.y / normal.y : -input.y / normal.y;
            else
                vectorY = input.y / Mathf.Abs(normal.x);

            return new Vector2(vectorX, vectorY);
        }
        
        public static Vector3 GetProjectedForward(Transform transform)
        {
            return transform.parent.InverseTransformDirection(
                Vector3.ProjectOnPlane(transform.forward, Vector3.up));
        }
    }
}