using TwoForksVr.Helpers;
using TwoForksVr.LaserPointer.Patches;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.LaserPointer
{
    public class Laser : MonoBehaviour
    {
        private const float laserLength = 1f;
        private LaserInputModule inputModule;
        private Transform laserTransform;
        private Transform leftHand;
        private LineRenderer lineRenderer;
        private Transform rightHand;
        private Vector3? target;

        public static Laser Create(Transform leftHand, Transform rightHand)
        {
            var instance = new GameObject("VrHandLaser").AddComponent<Laser>();
            var instanceTransform = instance.transform;
            instanceTransform.SetParent(rightHand, false);
            instanceTransform.localEulerAngles = new Vector3(39.132f, 356.9302f, 0.3666f);
            instance.rightHand = rightHand;
            instance.leftHand = leftHand;
            return instance;
        }

        public void SetUp(Camera camera)
        {
            inputModule.EventCamera = camera;
            target = null;
        }

        private void Start()
        {
            laserTransform = transform;
            PlayerTargetingPatches.LaserTransform = laserTransform;

            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.SetPositions(new[] {Vector3.zero, Vector3.forward * laserLength});
            lineRenderer.startWidth = 0.005f;
            lineRenderer.endWidth = 0.001f;
            lineRenderer.endColor = Color.white;
            lineRenderer.startColor = Color.white;
            lineRenderer.material.shader = Shader.Find("Particles/Alpha Blended Premultiply");
            lineRenderer.material.SetColor(ShaderProperty.Color, Color.white);
            lineRenderer.sortingOrder = 1000;
            lineRenderer.enabled = false;

            inputModule = LaserInputModule.Create(this);
        }

        private void Update()
        {
            UpdateLaserParent();
            UpdateLaserVisibility();
            UpdateLaserTarget();
        }

        public void SetTarget(Vector3? newTarget)
        {
            target = newTarget;
        }

        private void UpdateLaserTarget()
        {
            lineRenderer.SetPosition(1,
                target != null
                    ? transform.InverseTransformPoint((Vector3) target)
                    : Vector3.forward * laserLength);
        }

        private void UpdateLaserParent()
        {
            if (SteamVR_Actions.default_Interact.GetStateDown(SteamVR_Input_Sources.LeftHand))
                transform.SetParent(leftHand, false);
            if (SteamVR_Actions.default_Interact.GetStateDown(SteamVR_Input_Sources.RightHand))
                transform.SetParent(rightHand, false);
        }

        private bool HasCurrentTarget()
        {
            return vgHudManager.Instance && vgHudManager.Instance.currentTarget || target != null;
        }

        private void UpdateLaserVisibility()
        {
            lineRenderer.enabled =
                HasCurrentTarget() || SteamVR_Actions.default_Interact.state;
        }
    }
}