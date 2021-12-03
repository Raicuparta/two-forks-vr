using System;
using UnityEngine;

namespace TwoForksVr.UI
{
    public class AttachToCamera : MonoBehaviour
    {
        private const float offset = 3f;
        private static Transform cameraTransform;
        private static Action onTargetCameraSet;

        private void Awake()
        {
            onTargetCameraSet += HandleTargetCameraSet;
        }

        private void OnDestroy()
        {
            onTargetCameraSet -= HandleTargetCameraSet;
        }

        private void HandleTargetCameraSet()
        {
            UpdateTransform();
        }

        private void OnEnable()
        {
            UpdateTransform();
        }

        private void Start()
        {
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (!cameraTransform) return;
            var targetPosition = cameraTransform.position;
            var forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up);
            transform.position = targetPosition + forward * offset;
            transform.LookAt(2 * transform.position - targetPosition);
        }

        public static void SetTargetCamera(Camera camera)
        {
            cameraTransform = camera ? camera.transform : null;
            onTargetCameraSet();
        }
    }
}
