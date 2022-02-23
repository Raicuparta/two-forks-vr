using UnityEngine;

namespace TwoForksVr.Tools
{
    public class VrTrackingDevice : MonoBehaviour
    {
        public static VrTrackingDevice Create(vgTrackingDeviceController trackingDeviceController)
        {
            var instance = new GameObject("VrTrackingDeviceFakePlayerGameObject").AddComponent<VrTrackingDevice>();
            instance.transform.SetParent(trackingDeviceController.transform, false);

            return instance;
        }

        private void Update()
        {
            var forward = Vector3.ProjectOnPlane(transform.parent.right, Vector3.up);
            transform.position = transform.parent.position + Vector3.right * 0.1f;
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
    }
}