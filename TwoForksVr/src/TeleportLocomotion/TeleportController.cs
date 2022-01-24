using System;
using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.Limbs;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.TeleportLocomotion
{
    public class TeleportController: MonoBehaviour
    {
        private static readonly SteamVR_Action_Boolean teleportInput = SteamVR_Actions.default_Teleport;

        private TeleportArc teleportArc;
        private VrLimbManager limbManager;
        private static Transform teleportTarget; // todo no static, no public
        private static vgPlayerNavigationController navigationController; // todo no static

        public static TeleportController Create(VrStage stage, VrLimbManager limbManager)
        {
            var instance = stage.gameObject.AddComponent<TeleportController>();
            instance.limbManager = limbManager;
            teleportTarget = Instantiate(VrAssetLoader.TeleportTargetPrefab, instance.transform, false).transform;
            instance.teleportArc = TeleportArc.Create(instance, teleportTarget.GetComponentInChildren<Renderer>().material);
            return instance;
        }

        public void SetUp(Transform playerTransform)
        {
            if (playerTransform)
            {
                navigationController = playerTransform.GetComponent<vgPlayerNavigationController>();
            }
        }

        public static bool IsNextToTeleportMarker(Transform transform, float minSquareDistance = 0.3f)
        {
            if (!teleportTarget.gameObject.activeInHierarchy) return false;
            var targetPoint = teleportTarget.position;
            targetPoint.y = transform.position.y;
            var squareDistance = Vector3.SqrMagnitude(targetPoint - transform.position);
            return squareDistance < minSquareDistance;
        }
        
        public static bool IsTeleporting()
        {
            return VrSettings.Teleport.Value && SteamVR_Actions.default_Teleport.state && navigationController && navigationController.enabled;
        }

        private void Update()
        {
            if (teleportInput.GetStateDown(SteamVR_Input_Sources.LeftHand))
            {
                Logs.LogInfo("########### setting left hand teleport");
                SetHand(limbManager.LeftHand);
            }
            if (teleportInput.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                Logs.LogInfo("########### setting right hand teleport");
                SetHand(limbManager.RightHand);
            }
            UpdateArc();
            UpdatePlayerRotation();
        }

        private void UpdatePlayerRotation()
        {
            if (!IsTeleporting()) return;
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