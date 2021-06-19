using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Raicuparta.TwoForksVR
{
    public class VRMap: MonoBehaviour
    {
        private void Start()
        {
            // Very hard to read the map in VR since we can't zoom in.
            // So making it bigger to make it easier, especially for lower resolution headsets.
            transform.localScale = Vector3.one * 150f;

            // Need to disable and re-enable the cloth component to reset it,
            // otherwise the physics would be all fucky after resizing.
            var cloth = gameObject.GetComponent<Cloth>();
            cloth.enabled = false;
            cloth.enabled = true;
        }

        [HarmonyPatch(typeof(vgMapManager), "Awake")]
        public class PatchMapManagerAwake
        {
            public static void Prefix(ref RenderTexture ___lowResRenderTarget, RenderTexture ___highResRenderTarget)
            {
                // Forces the map to stay at high resolution, even when not zoomed,
                // since there's no "zoom" action in VR.
                ___lowResRenderTarget = ___highResRenderTarget;
            }
        }
    }
}
