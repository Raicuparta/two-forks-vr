using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Raicuparta.TwoForksVR
{
    [HarmonyPatch(typeof(vgMapManager), "Awake")]
    public class PatchMapManagerAwake
    {
        public static void Prefix(ref RenderTexture ___lowResRenderTarget, RenderTexture ___highResRenderTarget)
        {
            ___lowResRenderTarget = ___highResRenderTarget;
        }
    }
}
