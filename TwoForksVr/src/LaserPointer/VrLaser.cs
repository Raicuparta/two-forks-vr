using TwoForksVr.Helpers;
using TwoForksVr.VrInput;
using UnityEngine;
using Valve.VR;

namespace TwoForksVr.LaserPointer
{
    public class VrLaser : MonoBehaviour
    {
        private const float laserLength = 1f;
        private readonly SteamVR_Action_Boolean inputAction = BindingsManager.ActionSet.Interact;
        private bool ignoreNextInput;

        private LaserInputModule inputModule;
        private Transform leftHand;
        private LineRenderer lineRenderer;
        private Transform rightHand;
        private Vector3? target;

        public static VrLaser Create(Transform leftHand, Transform rightHand)
        {
            var instance = new GameObject("VrHandLaser").AddComponent<VrLaser>();
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
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.SetPositions(new[] {Vector3.zero, Vector3.forward * laserLength});
            lineRenderer.startWidth = 0.005f;
            lineRenderer.endWidth = 0.001f;
            lineRenderer.endColor = new Color(1, 1, 1, 0.8f);
            lineRenderer.startColor = Color.clear;
            lineRenderer.material.shader = Shader.Find("Particles/Alpha Blended Premultiply");
            lineRenderer.material.SetColor(ShaderProperty.Color, Color.white);
            lineRenderer.sortingOrder = 10000;
            lineRenderer.enabled = false;

            inputModule = LaserInputModule.Create(this);
        }

        private void Update()
        {
            UpdateLaserVisibility();
            UpdateLaserTarget();
            UpdateLaserParent(SteamVR_Input_Sources.LeftHand, leftHand);
            UpdateLaserParent(SteamVR_Input_Sources.RightHand, rightHand);
        }

        private void UpdateLaserParent(SteamVR_Input_Sources inputSource, Transform hand)
        {
            if (!inputAction.GetStateDown(inputSource) || transform.parent == hand) return;
            ignoreNextInput = true;
            transform.SetParent(hand, false);
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

        private bool HasCurrentTarget()
        {
            return vgHudManager.Instance && vgHudManager.Instance.currentTarget || target != null;
        }

        private void UpdateLaserVisibility()
        {
            lineRenderer.enabled =
                HasCurrentTarget() || BindingsManager.ActionSet.Interact.state;
        }

        public bool ClickDown()
        {
            if (ignoreNextInput) return false;
            return inputAction.stateDown;
        }

        public bool ClickUp()
        {
            if (ignoreNextInput)
            {
                ignoreNextInput = false;
                return false;
            }

            return inputAction.stateUp;
        }

        public bool IsClicking()
        {
            return inputAction.state;
        }
    }
}