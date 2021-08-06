using UnityEngine;

namespace TwoForksVR.UI
{
    public class AttachToCamera : MonoBehaviour
    {
        private const float offset = 0.5f;
        public Transform target;

        private void LateUpdate()
        {
            if (target == null)
            {
                target = Camera.main?.transform;
                return;
            }
            transform.position = target.position + target.forward * offset;
            transform.LookAt(2 * transform.position - target.position);
        }
    }
}
