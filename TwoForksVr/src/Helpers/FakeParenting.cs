using System;
using TwoForksVr.PlayerBody;
using UnityEngine;

namespace TwoForksVr.Helpers
{
    public delegate void UpdateEventHandler();

    // This component is useful when we need to simulate object parenting,
    // without actually changing the hierarchy.
    // TODO: this is confusing, the update is being called from RoomScaleBodyTransform,
    // to make sure it happens after body movement and rotation.
    public class FakeParenting : MonoBehaviour
    {
        public Transform Target;
        
        public static event UpdateEventHandler UpdateEvent;

        private void Awake()
        {
            UpdateEvent += UpdateTransform;
        }

        private void OnDestroy()
        {
            UpdateEvent -= UpdateTransform;
        }

        private void LateUpdate()
        {
            if (RoomScaleBodyTransform.Instance) return;
            UpdateTransform();
        }

        public static void InvokeUpdate()
        {
            UpdateEvent?.Invoke();
        }

        private void UpdateTransform()
        {
            if (!Target) return;
            transform.position = Target.position;
            transform.rotation = Target.rotation;
        }
    }
}