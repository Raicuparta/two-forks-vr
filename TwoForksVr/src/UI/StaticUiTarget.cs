using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.UI
{
    // This is an invisible object that's always(ish) somewhere in front of the player.
    // To be used as the position for UI elements that need to be visible or interacted with.
    public class StaticUiTarget : MonoBehaviour
    {
        private const float forwardOffset = 1f;
        private const float rotationSmoothTime = 0.3f;
        private const float minAngleDelta = 15f;
        private Transform cameraTransform;
        private Vector3 previousForward;
        private Quaternion rotationVelocity;
        private Quaternion targetRotation;
        private Transform targetTransform;

        public static StaticUiTarget Create(VrStage stage)
        {
            var instance = new GameObject(nameof(StaticUiTarget)).AddComponent<StaticUiTarget>();
            instance.transform.SetParent(stage.transform, false);
            instance.targetTransform = new GameObject("InteractiveUiTargetTransform").transform;
            instance.targetTransform.SetParent(instance.transform, false);
            instance.targetTransform.localPosition = new Vector3(0, 0, 1f);
            StaticUi.SetTargetTransform(instance.targetTransform);
            return instance;
        }

        public void SetUp(Camera camera)
        {
            if (!camera) return;
            cameraTransform = camera.transform;
            previousForward = cameraTransform.forward;
        }

        private void Update()
        {
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (!cameraTransform) return;

            var cameraForward = cameraTransform.forward;
            var unsignedAngleDelta = Vector3.Angle(previousForward, cameraForward);

            if (unsignedAngleDelta > minAngleDelta)
            {
                targetRotation = Quaternion.LookRotation(cameraForward);
                previousForward = cameraForward;
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