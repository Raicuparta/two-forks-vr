using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

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

            //var texture = TextureLoader.LoadPNG(@"C:\Users\rai\Repos\FirewatchCode\ExportedAssets\Texture2D\TrailheadMap - HighRez.png");

            //var material = gameObject.GetComponent<SkinnedMeshRenderer>().material;
            //material.mainTexture = texture;

            var mapBase = GameObject.Find("MapBase");

            var tex = Raicuparta.TwoForksVR.TextureLoader.LoadSprite(@"C:\Users\rai\Repos\two-forks-vr\TwoForksVR\Items\edited-map.png");

            mapBase.GetComponent<Image>().overrideSprite = tex;

        }

        [HarmonyPatch(typeof(vgMapManager), "Awake")]
        public class PatchMapManagerAwake
        {
            public static void Prefix(ref RenderTexture ___lowResRenderTarget, ref RenderTexture ___highResRenderTarget)
            {
                // Forces the map to stay at high resolution, even when not zoomed,
                // since there's no "zoom" action in VR.
                ___lowResRenderTarget = ___highResRenderTarget;

                ___lowResRenderTarget.Release();
                ___highResRenderTarget.Release();
                ___lowResRenderTarget = ___highResRenderTarget = new RenderTexture(4096, 4096, 24);
            }
        }
    }
}
