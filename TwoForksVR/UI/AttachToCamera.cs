using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwoForksVR.PlayerCamera;
using UnityEngine;

namespace TwoForksVR.UI
{
    public class AttachToCamera : MonoBehaviour
    {
        private float offset = 0.5f;
        public Transform target;

        private void LateUpdate ()
        {
            if (target == null)
            {
                target = VRCameraManager.Instance?.VRCamera?.transform;
                return;
            }
            transform.position = target.position + target.forward * offset;
            transform.LookAt(2 * transform.position - target.position);
        }
    }
}
