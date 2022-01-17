using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.UI
{
    // This is an invisible object that's always(ish) somewhere in front of the player.
    // To be used as the position for UI elements that need to be visible or interacted with.
    public class InteractiveUiTarget : MonoBehaviour
    {
        private const float offset = 3f;
        private Transform cameraTransform;
        private Transform targetTransform;

        private void Update()
        {
            UpdateTransform();
        }

        public static InteractiveUiTarget Create(VrStage stage)
        {
            var instance = new GameObject(nameof(InteractiveUiTarget)).AddComponent<InteractiveUiTarget>();
            instance.transform.SetParent(stage.transform, false);
            instance.targetTransform = new GameObject("InteractiveUiTargetTransform").transform;
            instance.targetTransform.SetParent(instance.transform, false);
            instance.targetTransform.localPosition = Vector3.forward * offset;
            InteractiveUi.SetTargetTransform(instance.targetTransform);
            return instance;
        }

        public void SetUp(Camera camera)
        {
            if (!camera) return;
            cameraTransform = camera.transform;
            prevForward = MathHelper.GetProjectedForward(cameraTransform);
        }

        private Vector3 prevForward;
        private Quaternion targetRotation;
        private Quaternion rotationVelocity;
        private const float rotationSmoothTime = 0.3f;
        private const float minAngleDelta = 45f;

        private void UpdateTransform()
        {
            if (!cameraTransform) return;

            var cameraForward = MathHelper.GetProjectedForward(cameraTransform);
            var unsignedAngleDelta = Vector3.Angle(prevForward, cameraForward);

            if (unsignedAngleDelta > minAngleDelta)
            {
                targetRotation = Quaternion.LookRotation(cameraForward);
                prevForward = cameraForward;
            }

            transform.rotation = MathHelper.SmoothDamp(
                transform.rotation,
                targetRotation,
                ref rotationVelocity,
                rotationSmoothTime);

            transform.position = cameraTransform.position;
        }
    }
}