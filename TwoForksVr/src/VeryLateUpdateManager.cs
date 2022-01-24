using TwoForksVr.Helpers;
using TwoForksVr.PlayerBody;
using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr
{
    // This manager handles some methods that need to run as late as possible, after every other update.
    // They often need to run in a specific order, which is also defined here.
    public class VeryLateUpdateManager: MonoBehaviour
    {
        private Camera camera;
        private static RoomScaleBodyTransform roomScaleBodyTransform;

        public static VeryLateUpdateManager Create(VrStage stage)
        {
            return stage.gameObject.AddComponent<VeryLateUpdateManager>();
        }

        public void SetUp(Camera activeCamera)
        {
            camera = activeCamera;
        }

        private void Awake()
        {
            Camera.onPreCull += HandlePreCull;
        }

        private void OnDestroy()
        {
            Camera.onPreCull -= HandlePreCull;
        }

        public static void SetRoomScaleBodyTransform(RoomScaleBodyTransform newRoomScaleBodyTransform)
        {
            roomScaleBodyTransform = newRoomScaleBodyTransform;
        }

        private static void InvokeVeryLateUpdate(TwoForksVrBehavior behavior)
        {
            if (!behavior || !behavior.enabled) return;
            behavior.VeryLateUpdate();
        }

        private void HandlePreCull(Camera preCullCamera)
        {
            if (preCullCamera != camera) return;

            InvokeVeryLateUpdate(roomScaleBodyTransform);
            FakeParenting.InvokeUpdate();
        }
    }
}