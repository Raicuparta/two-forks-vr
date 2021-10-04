using HarmonyLib;
using TwoForksVR.Stage;

namespace TwoForksVR.PlayerCamera.Patches
{
    [HarmonyPatch]
    public class GameCameraPatches
    {
        private static bool isDone;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.LateUpdate))]
        private static void RecenterCamera()
        {
            if (isDone) return;
            VRStage.Instance.Recenter();
            isDone = true;
        }
        
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.Start))]
        [HarmonyPrefix]
        private static void ResetIsDoneOnCameraStart()
        {
            isDone = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgLoadingCamera), nameof(vgLoadingCamera.OnEnable))]
        private static void ResetIsDoneOnLoading()
        {
            isDone = false;
        }
    }
}