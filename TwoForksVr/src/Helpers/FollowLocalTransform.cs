using UnityEngine;

namespace TwoForksVr.Helpers
{
    public class FollowLocalTransform: MonoBehaviour
    {
        public Transform Target;

        private void LateUpdate()
        {
            transform.localRotation = Target.localRotation;
            transform.localRotation = Target.localRotation;
            transform.localScale = Target.localScale;
        }
    }
}