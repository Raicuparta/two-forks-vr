using MelonLoader;
using System;
using System.Reflection;
using UnityEngine;

namespace Raicuparta.UnityVRCameraReparent
{
    public class UnityVRCameraReparent : MelonMod
    {
        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                ReparentCamera();
            }
        }

        private void ReparentCamera()
        {
            MelonLogger.Msg("Reparenting camera");

            var mainCamera = Camera.main.transform;
            var vrCameraParent = new GameObject().transform;
            vrCameraParent.SetParent(mainCamera.parent, false);
            mainCamera.SetParent(vrCameraParent);
            vrCameraParent.localPosition = Vector3.down;
        }
    }
}
