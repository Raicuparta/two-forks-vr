using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.UI
{
    public class StaticUiTarget : UiTarget
    {
        protected override float MinAngleDelta => 15f;

        public static StaticUiTarget Create(VrStage stage)
        {
            var instance = new GameObject(nameof(StaticUiTarget)).AddComponent<StaticUiTarget>();
            instance.transform.SetParent(stage.transform, false);
            instance.TargetTransform = new GameObject("InteractiveUiTargetTransform").transform;
            instance.TargetTransform.SetParent(instance.transform, false);
            instance.TargetTransform.localPosition = Vector3.forward;
            StaticUi.SetTargetTransform(instance.TargetTransform);
            return instance;
        }

        protected override Vector3 GetCameraForward()
        {
            return CameraTransform.forward;
        }
    }
}