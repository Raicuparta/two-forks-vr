using UnityEngine;

namespace TwoForksVr.Helpers
{
    public class FollowLocalTransform : TwoForksVrBehavior
    {
        public Transform Target;

        protected override void VeryLateUpdate()
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