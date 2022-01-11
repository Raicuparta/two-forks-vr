using System;
using UnityEngine;

namespace TwoForksVr.Stage
{
    // This is an invisible object that's always(ish) somewhere in front of the player.
    // To be used as the position for UI elements that need to be visible or interacted with.
    public class MenuFollowTarget: MonoBehaviour
    {
        private float maxSquareDistance = 3f;
        private float smoothTime = 0.3F;
        private Vector3 velocity = Vector3.zero;
        private Vector3? currentTarget;
        private Transform cameraTransform;
        private const float offset = 3f;
        public static MenuFollowTarget Instance; // TODO not public static
    
        public static MenuFollowTarget Create(VrStage stage)
        {
            Instance = new GameObject("MenuFollowTarget").AddComponent<MenuFollowTarget>();
            Instance.transform.SetParent(stage.transform, false);
            return Instance;
        }

        // private void Start()
        // {
        //     var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //     cube.transform.SetParent(transform, false);
        // }

        public void SetUp(Camera camera)
        {
            cameraTransform = camera ? camera.transform : null;
        }

        private void Update()
        {
            UpdateTransform();
        }

        private Vector3 GetTargetPosition()
        {
            if (!cameraTransform) return Vector3.zero;

            var cameraPosition = cameraTransform.position;
            var forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            return cameraPosition + forward * offset;
        }

        private void UpdateTransform()
        {
            if (!cameraTransform) return;
            var targetThisFrame = GetTargetPosition();

            var squareDistance = Vector3.SqrMagnitude(targetThisFrame - transform.position);

            currentTarget = (squareDistance > maxSquareDistance || currentTarget == null) ? targetThisFrame : currentTarget;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                currentTarget ?? targetThisFrame,
                ref velocity,
                smoothTime,
                float.PositiveInfinity,
                Time.unscaledDeltaTime);
            transform.LookAt(2 * transform.position - cameraTransform.position);
        }
    }
}