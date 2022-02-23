using UnityEngine;

namespace TwoForksVr.Helpers
{
    public class CopyLocalTransformValues : TwoForksVrBehavior
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
            transform.localPosition = Target.localPosition;
            transform.localScale = Target.localScale;
        }
    }
}