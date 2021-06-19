using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Raicuparta.TwoForksVR
{
    class VRUIManager: MonoBehaviour
    {
        private void Start()
        {
            SetUpUI();
        }

        private void SetUpUI()
        {
            var canvases = GameObject.FindObjectsOfType<Canvas>().Where(canvas => canvas.renderMode == RenderMode.ScreenSpaceOverlay);
            canvases.Do(canvas =>
            {
                if (canvas.name == "BlackBars")
                {
                    canvas.enabled = false;
                    return;
                }
                canvas.worldCamera = Camera.main;
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.transform.SetParent(Camera.main.transform, false);
                canvas.transform.localPosition = Vector3.forward * 0.5f;
                canvas.transform.localScale = Vector3.one * 0.0004f;
            });
        }

        [HarmonyPatch(typeof(vgScrimManager), "ShowScrim")]
        public class PatchScrimManagerShow
        {
            public static void Prefix(ref bool blur)
            {
                blur = false;
            }
        }
    }
}
