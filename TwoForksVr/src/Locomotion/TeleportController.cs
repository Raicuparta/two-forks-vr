using System;
using TwoForksVr.Assets;
using TwoForksVr.Limbs;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using TwoForksVr.VrInput.ActionInputs;
using UnityEngine;

namespace TwoForksVr.Locomotion;

public class TeleportController : MonoBehaviour
{
    private const float triggerTeleportSquareDistance = 0.3f;
    private static readonly IActionInput teleportInput = ActionInputDefinitions.Teleport;
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
        return instance;
    }

    public void SetUp(vgPlayerController playerController)
    {
        navigationController =
            playerController ? playerController.GetComponent<vgPlayerNavigationController>() : null;
    }

    private void Start()
    {
        UpdateHand();
    }

    private void Update()
    {
        UpdateArc();
        UpdatePlayerRotation();
    }

    private void LateUpdate()
    {
        if (!navigationController || !VrSettings.Teleport.Value || !navigationController.enabled) return;

        navigationController.playerController.forwardInput = IsTeleporting() ? 1 : 0;
    }

    private void OnEnable()
    {
        VrSettings.Config.SettingChanged += SwapSticksOnSettingChanged;
    }

    private void OnDisable()
    {
        VrSettings.Config.SettingChanged -= SwapSticksOnSettingChanged;
    }

    private void SwapSticksOnSettingChanged(object sender, EventArgs e)
    {
        UpdateHand();
    }

    public bool IsNextToTeleportMarker(Transform objectTransform)
    {
        if (!teleportTarget.gameObject.activeInHierarchy) return false;
        var targetPoint = teleportTarget.position;
        targetPoint.y = objectTransform.position.y;
        var squareDistance = Vector3.SqrMagnitude(targetPoint - objectTransform.position);
        return squareDistance < triggerTeleportSquareDistance;
    }

    public bool IsTeleporting()
    {
        return VrSettings.Teleport.Value &&
               teleportInput.ButtonValue && navigationController &&
               navigationController.enabled && !vgPauseManager.Instance.isPaused;
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

    private void UpdateHand()
    {
        teleportArc.transform.SetParent(limbManager.GetMovementStickHand().transform, false);
    }
}