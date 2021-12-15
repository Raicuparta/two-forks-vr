using System;
using UnityEngine;

namespace TwoForksVr.UI
{
    public abstract class AttachToCamera : MonoBehaviour
    {
        protected static Transform CameraTransform;

        private static Action onTargetCameraSet;

        private void Awake()
        {
            onTargetCameraSet += HandleTargetCameraSet;
        }

        private void OnDestroy()
        {
            onTargetCameraSet -= HandleTargetCameraSet;
        }

        protected abstract void HandleTargetCameraSet();

        public static void SetTargetCamera(Camera camera)
        {
            CameraTransform = camera ? camera.transform : null;
            onTargetCameraSet?.Invoke();
        }
    }
}
