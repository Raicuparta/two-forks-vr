using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.UI
{
    // This is an invisible object that's always(ish) somewhere in front of the player.
    // To be used as the position for UI elements that need to be visible or interacted with.
    public class InteractiveUiTarget : MonoBehaviour
    {
        private const float maxSquareDistance = 3f;
        private const float smoothTime = 0.3F;
        private const float offset = 3f;
        private Transform cameraTransform;
        private Vector3? currentTarget;
        private Vector3 velocity = Vector3.zero;

        private void Update()
        {
            UpdateTransform();
        }

        public static InteractiveUiTarget Create(VrStage stage)
        {
            var instance = new GameObject(nameof(InteractiveUiTarget)).AddComponent<InteractiveUiTarget>();
            instance.transform.SetParent(stage.transform, false);
            InteractiveUi.SetTargetTransform(instance.transform);
            return instance;
        }

        public void SetUp(Camera camera)
        {
            cameraTransform = camera ? camera.transform : null;
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

            currentTarget = squareDistance > maxSquareDistance || currentTarget == null
                ? targetThisFrame
                : currentTarget;

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