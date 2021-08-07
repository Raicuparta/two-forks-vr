using UnityEngine;

namespace TwoForksVR.UI
{
    public class AttachToCamera : MonoBehaviour
    {
        private float offset = 0.8f;
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
