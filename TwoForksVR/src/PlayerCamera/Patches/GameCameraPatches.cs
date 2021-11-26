using HarmonyLib;
using TwoForksVr.Stage;

namespace TwoForksVr.PlayerCamera.Patches
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
            VRStage.Instance.Recenter(true);
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
