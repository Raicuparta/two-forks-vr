using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.UI
{
    public class AttachedUi : MonoBehaviour
    {
        private Transform targetTransform;

        public static void Create<TAttachedUi>(Canvas canvas, Transform target, float scale = 0)
            where TAttachedUi : AttachedUi
        {
            var instance = canvas.gameObject.AddComponent<TAttachedUi>();
            if (scale > 0) canvas.transform.localScale = Vector3.one * scale;
            canvas.renderMode = RenderMode.WorldSpace;

            instance.targetTransform = target;
        }

        protected virtual void Update()
        {
            if (!targetTransform)
            {
                Logs.LogWarning($"Target transform for AttachedUi {name} is missing, destroying");
                Destroy(this);
                return;
            }

            UpdateTransform();
        }

        public void SetTargetTransform(Transform target)
        {
            targetTransform = target;
        }

        private void UpdateTransform()
        {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }
    }
}