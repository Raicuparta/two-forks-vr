using TwoForksVr.Helpers;
using TwoForksVr.Locomotion;
using TwoForksVr.Settings;
using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.PlayerBody;

public class RoomScaleBodyTransform : TwoForksVrBehavior
{
    private const float minPositionOffset = 0.05f;
    private const float maxPositionOffset = 1f;

    private Transform cameraTransform;
    private CharacterController characterController;
    private vgPlayerNavigationController navigationController;
    private Vector3 prevCameraPosition;
    private Vector3 prevForward;
    private bool previousNavigationControlerEnabled;
    private VrStage stage;
    private TeleportController teleportController;

    public static RoomScaleBodyTransform Create(VrStage stage, TeleportController teleportController)
    {
        var instance = stage.gameObject.AddComponent<RoomScaleBodyTransform>();
        instance.teleportController = teleportController;
        instance.stage = stage;
        return instance;
    }

    public void SetUp(vgPlayerController playerController)
    {
        if (!playerController) return;
        cameraTransform = playerController.playerCamera.transform;
        characterController = playerController.characterController;
        navigationController = playerController.navController;
        prevCameraPosition = cameraTransform.position;
        prevForward = GetCameraForward();
    }

    private void Update()
    {
        UpdateRecenterOnEnablingNavigationController();
    }

    protected override void VeryLateUpdate()
    {
        if (ShouldSkipUpdate()) return;
        UpdateRotation();

        if (VrSettings.RoomScaleBodyPosition.Value)
        {
            UpdateRoomScalePosition();
        }
    }

    // The navigation controller gets disabled while some larger animations are playing.
    // Room scale body transform adjustments are paused while the navigation controller is disabled.
    // So we recenter the position and rotation when the navigation controller is enabled again.
    private void UpdateRecenterOnEnablingNavigationController()
    {
        if (!navigationController) return;

        // Recentering immediately after animations didn't always work well for some animations,
        // so instead I'm recentering with a small delay.
        if (HasNavigatorStateChanged()) Invoke(nameof(Recenter), 0.1f);

        previousNavigationControlerEnabled = navigationController.enabled;
    }

    private void Recenter()
    {
        stage.RecenterPosition(true);
        stage.RecenterRotation();
    }

    private bool HasNavigatorStateChanged()
    {
        if (VrSettings.FixedCameraDuringAnimations.Value)
            return navigationController.enabled != previousNavigationControlerEnabled;
        return navigationController.enabled && !previousNavigationControlerEnabled;
    }

    private bool ShouldSkipUpdate()
    {
        return !characterController || teleportController && teleportController.IsTeleporting();
    }

    private void UpdateRoomScalePosition()
    {
        var cameraPosition = cameraTransform.localPosition;

        var localPositionDelta = cameraPosition - prevCameraPosition;
        localPositionDelta.y = 0;

        var cameraToPlayer = cameraTransform.position - characterController.transform.position;
        cameraToPlayer.y = 0;
        var cameraPlayerDistance = cameraToPlayer.sqrMagnitude;

        var worldPositionDelta = stage.transform.TransformVector(localPositionDelta);


        prevCameraPosition = cameraPosition;

        // TODO: Min movement threshold isn't workinf if player walks in real life, moving against a game wall.
        if (cameraPlayerDistance < minPositionOffset || !navigationController.onGround ||
            !navigationController.enabled) return;

        if (worldPositionDelta.sqrMagnitude > maxPositionOffset) return;

        var groundedPositionDelta = Vector3.ProjectOnPlane(worldPositionDelta, navigationController.groundNormal);

        var playerPositionBefore = characterController.transform.position;

        characterController.Move(groundedPositionDelta);

        var playerPositionAfter = characterController.transform.position;

        // There's a chance this might break some other movement-related stuff,
        // like resetting animations.
        navigationController.positionLastFrame = playerPositionAfter;

        stage.transform.position -= playerPositionAfter - playerPositionBefore;
    }

    private Vector3 GetCameraForward()
    {
        return cameraTransform.parent.InverseTransformDirection(MathHelper.GetProjectedForward(cameraTransform));
    }

    private void UpdateRotation()
    {
        if (!navigationController.onGround || !navigationController.enabled) return;

        var cameraForward = GetCameraForward();
        var angleDelta = MathHelper.SignedAngle(prevForward, cameraForward, Vector3.up);
        prevForward = cameraForward;
        characterController.transform.Rotate(Vector3.up, angleDelta);

        stage.RecenterRotation();
    }
}