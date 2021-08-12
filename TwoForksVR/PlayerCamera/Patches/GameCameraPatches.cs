using HarmonyLib;
using TwoForksVR.Stage;

namespace TwoForksVR.PlayerCamera.Patches
{
    public class GameCameraPatches
    {
        private static bool isDone;

        [HarmonyPatch(typeof(vgCameraController), "LateUpdate")]
        public class RecenterCamera
        {
            public static void Postfix()
            {
                if (isDone) return;
                VRStage.Instance.Recenter();
                isDone = true;
            }
        }

        [HarmonyPatch(typeof(vgCameraController), "Start")]
        public class ResetIsDoneOnCameraStart
        {
            public static void Prefix()
            {
                isDone = false;
            }
        }

        [HarmonyPatch(typeof(vgLoadingCamera), "OnEnable")]
        public class ResetIsDoneOnLoading
        {
            public static void Prefix()
            {
                isDone = false;
            }
        }
    }
}