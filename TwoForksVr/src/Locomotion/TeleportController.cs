using TwoForksVr.Assets;
using TwoForksVr.Limbs;
using TwoForksVr.Locomotion.Patches;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.Locomotion
{
    public class TeleportController : MonoBehaviour
    {
        private static readonly SteamVR_Action_Boolean teleportInput = SteamVR_Actions.default_Teleport;
        private VrLimbManager limbManager;
        private vgPlayerNavigationController navigationController;

        private TeleportArc teleportArc;
        private Transform teleportTarget;

        public static TeleportController Create(VrStage stage, VrLimbManager limbManager)
        {
            var instance = stage.gameObject.AddComponent<TeleportController>();
            instance.limbManager = limbManager;
            instance.teleportTarget =
                Instantiate(VrAssetLoader.TeleportTargetPrefab, instance.transform, false).transform;
            instance.teleportArc = TeleportArc.Create(instance,
                instance.teleportTarget.GetComponentInChildren<Renderer>().material);
            TeleportLocomotionPatches.Teleport = instance;
            return instance;
        }

        public void SetUp(vgPlayerController playerController)
        {
            navigationController =
                playerController ? playerController.GetComponent<vgPlayerNavigationController>() : null;
        }

        private void Update()
        {
            if (teleportInput.GetStateDown(SteamVR_Input_Sources.LeftHand)) SetHand(limbManager.LeftHand);
            if (teleportInput.GetStateDown(SteamVR_Input_Sources.RightHand)) SetHand(limbManager.RightHand);
            UpdateArc();
            UpdatePlayerRotation();
        }

        public bool IsNextToTeleportMarker(Transform objectTransform, float minSquareDistance = 0.3f)
        {
            if (!teleportTarget.gameObject.activeInHierarchy) return false;
            var targetPoint = teleportTarget.position;
            targetPoint.y = objectTransform.position.y;
            var squareDistance = Vector3.SqrMagnitude(targetPoint - objectTransform.position);
            return squareDistance < minSquareDistance;
        }

        public bool IsTeleporting()
        {
            return VrSettings.Teleport.Value && SteamVR_Actions.default_Teleport.state && navigationController &&
                   navigationController.enabled;
        }

        private void UpdatePlayerRotation()
        {
            if (!IsTeleporting() || !navigationController) return;
            var targetPoint = teleportTarget.position;
            targetPoint.y = navigationController.transform.position.y;
            navigationController.transform.LookAt(targetPoint, Vector3.up);
        }

        private void UpdateArc()
        {
            if (!IsTeleporting())
            {
                teleportTarget.gameObject.SetActive(false);
                teleportArc.Hide();
                return;
            }

            teleportArc.Show();

            var hit = teleportArc.DrawArc(out var hitInfo);
            if (hit)
            {
                teleportTarget.gameObject.SetActive(true);
                teleportTarget.position = hitInfo.point;
            }
            else
            {
                teleportTarget.gameObject.SetActive(false);
            }
        }

        private void SetHand(VrHand hand)
        {
            teleportArc.transform.SetParent(hand.transform, false);
        }
    }
}