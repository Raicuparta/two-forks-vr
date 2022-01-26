using UnityEngine;

namespace TwoForksVr.UI
{
    public class StaticUi : MonoBehaviour
    {
        private static Transform targetTransform;

        private void Update()
        {
            UpdateTransform();
        }

        public static void SetTargetTransform(Transform transform)
        {
            targetTransform = transform;
        }

        private void UpdateTransform()
        {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }
    }
}