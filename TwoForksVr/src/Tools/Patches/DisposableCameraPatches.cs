using HarmonyLib;
using TwoForksVr.Helpers;
using TwoForksVr.Limbs;
using UnityEngine;

namespace TwoForksVr.Tools.Patches
{
    [HarmonyPatch]
    public static class DisposableCameraPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgPlayerAttachment), nameof(vgPlayerAttachment.SetActive))]
        private static void MoveDisposableCameraCounterToCamera(vgPlayerAttachment __instance, bool active)
        {
            if (!active || __instance.name != "DisposableCamera") return;

            var attachment = __instance.attachmentInstance;

            var remainingShotsText = vgHudManager.Instance.disposableCameraShotDisplay;
            if (remainingShotsText == null)
            {
                Logs.LogError("Failed to find disposable camera remaining shots text");
                return;
            }

            var canvas = attachment.GetComponent<Canvas>();

            if (canvas == null)
            {
                canvas = attachment.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
            }

            remainingShotsText.transform.SetParent(canvas.transform);
            remainingShotsText.transform.localPosition = new Vector3(0.01f, 0.03f, -0.03f);
            remainingShotsText.transform.localRotation = Quaternion.identity;
            remainingShotsText.transform.localScale = Vector3.one * 0.0002f;

            VrHandednessXMirror.Create(attachment.transform);
        }
    }
}