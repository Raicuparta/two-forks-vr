using TwoForksVr.Limbs;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using TwoForksVr.VrCamera;
using TwoForksVr.VrInput.ActionInputs;
using UnityEngine;

namespace TwoForksVr.Locomotion
{
    public class TurningController : MonoBehaviour
    {
        private const float smoothRotationSpeed = 150f; // TODO make this configurable.
        private const float snapRotationAngle = 60f; // TODO make this configurable.
        private VrLimbManager limbManager;
        private vgPlayerNavigationController navigationController;
        private VrStage stage;
        private TeleportController teleportController;

        public static TurningController Create(VrStage stage, TeleportController teleportController,
            VrLimbManager limbManager)
        {
            var instance = stage.gameObject.AddComponent<TurningController>();
            instance.teleportController = teleportController;
            instance.stage = stage;
            instance.limbManager = limbManager;
            return instance;
        }

        public void SetUp(vgPlayerController playerController)
        {
            navigationController =
                playerController ? playerController.GetComponent<vgPlayerNavigationController>() : null;
        }

        private void Update()
        {
            if (!navigationController || !navigationController.enabled || teleportController.IsTeleporting() ||
                limbManager.IsToolPickerOpen || vgPauseManager.Instance.isPaused) return;

            if (VrSettings.SnapTurning.Value)
                UpdateSnapTurning();
            else
                UpdateSmoothTurning();
        }

        private void UpdateSnapTurning()
        {
            if (ActionInputDefinitions.SnapTurnLeft.ButtonDown)
            {
                stage.FadeToBlack();
                Invoke(nameof(SnapTurnLeft), FadeOverlay.Duration);
            }

            if (ActionInputDefinitions.SnapTurnRight.ButtonDown)
            {
                stage.FadeToBlack();
                Invoke(nameof(SnapTurnRight), FadeOverlay.Duration);
            }
        }

        private void UpdateSmoothTurning()
        {
            navigationController.transform.Rotate(
                Vector3.up,
                ActionInputDefinitions.RotateX.AxisValue * smoothRotationSpeed * Time.unscaledDeltaTime);
        }

        private void SnapTurnLeft()
        {
            SnapTurn(-snapRotationAngle);
        }

        private void SnapTurnRight()
        {
            SnapTurn(snapRotationAngle);
        }

        private void SnapTurn(float angle)
        {
            navigationController.transform.Rotate(Vector3.up, angle);
            Invoke(nameof(EndSnap), FadeOverlay.Duration);
        }

        private void EndSnap()
        {
            stage.FadeToClear();
        }
    }
}