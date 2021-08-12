using UnityEngine;

namespace TwoForksVR.UI
{
    public class AttachToCamera : MonoBehaviour
    {
        public Transform target;
        private const float offset = 0.8f;

        private void LateUpdate()
        {
            if (target == null && Camera.main)
            {
                target = Camera.main.transform;
                return;
            }

            var targetPosition = target.position;
            transform.position = targetPosition + target.forward * offset;
            transform.LookAt(2 * transform.position - targetPosition);
        }
    }
}