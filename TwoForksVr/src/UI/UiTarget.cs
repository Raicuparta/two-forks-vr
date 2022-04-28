using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.UI;

// This is an invisible object that's always(ish) somewhere in front of the camera.
// To be used as the position for UI elements that need to be visible or interacted with.
public abstract class UiTarget : MonoBehaviour
{
    private const float rotationSmoothTime = 0.3f;
    protected Transform CameraTransform;
    private Vector3 previousForward;
    private Quaternion rotationVelocity;
    private Quaternion targetRotation;
    public Transform TargetTransform { get; protected set; }
    protected abstract float MinAngleDelta { get; }

    public void SetUp(Camera camera)
    {
        if (!camera) return;
        CameraTransform = camera.transform;
        previousForward = GetCameraForward();
    }

    protected virtual void Update()
    {
        UpdateTransform();
    }

    protected abstract Vector3 GetCameraForward();

    private void UpdateTransform()
    {
        if (!CameraTransform) return;

        var cameraForward = GetCameraForward();
        var unsignedAngleDelta = Vector3.Angle(previousForward, cameraForward);

        if (unsignedAngleDelta > MinAngleDelta)
        {
            targetRotation = Quaternion.LookRotation(cameraForward);
            previousForward = cameraForward;
        }

        transform.rotation = MathHelper.SmoothDamp(
            transform.rotation,
            targetRotation,
            ref rotationVelocity,
            rotationSmoothTime);

        transform.position = CameraTransform.position;
    }
}