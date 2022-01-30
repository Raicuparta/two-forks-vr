using System;
using UnityEngine;

namespace TwoForksVr.Helpers
{
    // This component is useful when we need to simulate object parenting,
    // without actually changing the hierarchy.
    public class FakeParenting : TwoForksVrBehavior
    {
        [Flags]
        public enum UpdateType
        {
            None = 0,
            LateUpdate = 1,
            VeryLateUpdate = 2
        }

        private Transform target;
        private UpdateType updateTypes;

        public static FakeParenting Create(Transform transform, Transform target = null,
            UpdateType updateType = UpdateType.VeryLateUpdate)
        {
            var instance = transform.gameObject.AddComponent<FakeParenting>();
            instance.target = target;
            instance.updateTypes = updateType;
            return instance;
        }

        private void LateUpdate()
        {
            if (!IsUpdateType(UpdateType.LateUpdate)) return;
            UpdateTransform();
        }

        protected override void VeryLateUpdate()
        {
            if (!IsUpdateType(UpdateType.VeryLateUpdate)) return;
            UpdateTransform();
        }

        private bool IsUpdateType(UpdateType type)
        {
            return (updateTypes & type) != UpdateType.None;
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