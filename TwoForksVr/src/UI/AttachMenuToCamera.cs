using TwoForksVr.Debugging;
using UnityEngine;

namespace TwoForksVr.UI
{
    public class AttachMenuToCamera : AttachToCamera
    {
        private const float offset = 3f;
        private BoxCollider collider;

        protected override void HandleTargetCameraSet()
        {
            UpdateTransform();
        }

        private void OnEnable()
        {
            UpdateTransform();
        }

        private void Start()
        {
            UpdateTransform();
            SetUpCollider();
        }
        
        private void SetUpCollider()
        {
            collider = gameObject.GetComponent<BoxCollider>();
            if (collider != null) return;

            var rectTransform = gameObject.GetComponent<RectTransform>();
            collider = gameObject.gameObject.AddComponent<BoxCollider>();
            var rectSize = rectTransform.sizeDelta;
            collider.size = new Vector3(rectSize.x, rectSize.y, 0.1f);
            gameObject.layer = LayerMask.NameToLayer("UI");
            gameObject.GetComponent<Canvas>().worldCamera = Camera.main; // TODO update this from cameratransform?
            
            gameObject.AddComponent<DebugCollider>();
        }

        private void UpdateTransform()
        {
            if (!CameraTransform) return;
            var targetPosition = CameraTransform.position;
            var forward = Vector3.ProjectOnPlane(CameraTransform.forward, Vector3.up);
            transform.position = targetPosition + forward * offset;
            transform.LookAt(2 * transform.position - targetPosition);
        }
    }
}
