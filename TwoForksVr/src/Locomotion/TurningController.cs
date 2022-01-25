using TwoForksVr.PlayerCamera;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.Locomotion
{
    public class TurningController: MonoBehaviour
    {
        private const float smoothRotationSpeed = 150f; // TODO make this configurable.
        private const float snapRotationAngle = 60f; // TODO make this configurable.
        private vgPlayerNavigationController navigationController;

        public static void Create(vgPlayerController playerController)
        {
            var instance = playerController.gameObject.AddComponent<TurningController>();
            instance.navigationController = playerController.navController;
        }

        private void Update()
        {
            if (!navigationController.onGround || !navigationController.enabled) return;
            
            if (VrSettings.SnapTurning.Value)
            {
                UpdateSnapTurning();
            }
            else
            {
                UpdateSmoothTurning();
            }
        }
        
        private void UpdateSnapTurning()
        {
            if (SteamVR_Actions.default_SnapTurnLeft.stateDown)
            {
                VrStage.Instance.FadeToBlack();
                Invoke(nameof(SnapTurnLeft), FadeOverlay.Duration);
            }
            if (SteamVR_Actions.default_SnapTurnRight.stateDown)
            {
                VrStage.Instance.FadeToBlack();
                Invoke(nameof(SnapTurnRight), FadeOverlay.Duration);
            }
        }

        private void UpdateSmoothTurning()
        {
            navigationController.transform.Rotate(
                Vector3.up,
                SteamVR_Actions._default.Rotate.axis.x * smoothRotationSpeed * Time.deltaTime);
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
            VrStage.Instance.FadeToClear();
        }
    }
}