using System;
using UnityEngine;

namespace TwoForksVr.Helpers
{
    public delegate void MyEventHandler();

    // This component is useful when we need to simulate object parenting,
    // without actually changing the hierarchy.
    public class FakeParenting : MonoBehaviour
    {
        public Transform Target;
        
        public static event MyEventHandler MyEvent;

        private void Awake()
        {
            MyEvent += HandlePreCull;
        }

        private void OnDestroy()
        {
            MyEvent -= HandlePreCull;
        }

        public static void InvokeEvent()
        {
            MyEvent?.Invoke();
        }

        private void HandlePreCull()
        {
            // TODO: ignore other cameras
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (!Target) return;
            transform.position = Target.position;
            transform.rotation = Target.rotation;
        }
    }
}