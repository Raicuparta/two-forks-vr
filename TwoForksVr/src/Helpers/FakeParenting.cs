using UnityEngine;

namespace TwoForksVr.Helpers
{
    // This component is useful when we need to simulate object parenting,
    // without actually changing the hierarchy.
    public class FakeParenting : TwoForksVrBehavior
    {
        public enum UpdateType
        {
            LateUpdate,
            VeryLateUpdate
        }

        private Transform target;
        private UpdateType updateType;

        public static FakeParenting Create(Transform transform, Transform target = null,
            UpdateType updateType = UpdateType.VeryLateUpdate)
        {
            var instance = transform.gameObject.AddComponent<FakeParenting>();
            instance.target = target;
            instance.updateType = updateType;
            return instance;
        }

        private void LateUpdate()
        {
            if (updateType != UpdateType.LateUpdate) return;
            UpdateTransform();
        }

        protected override void VeryLateUpdate()
        {
            if (updateType != UpdateType.VeryLateUpdate) return;
            UpdateTransform();
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void UpdateTransform()
        {
            if (!target) return;
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}