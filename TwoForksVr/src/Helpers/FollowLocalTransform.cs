using UnityEngine;

namespace TwoForksVr.Helpers
{
    public class FollowLocalTransform: MonoBehaviour
    {
        public Transform Target;

        private void LateUpdate()
        {
            if (!Target)
            {
                Destroy(this);
                return;
            }
            transform.localRotation = Target.localRotation;
            transform.localRotation = Target.localRotation;
            transform.localScale = Target.localScale;
        }
    }
}