using HarmonyLib;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

namespace TwoForksVr.Stage.Patches
{
    [HarmonyPatch]
    public class StagePatches : TwoForksVrPatch
    {
        private static vgReset resetObject;
        private static Transform originalParent;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgReset), nameof(vgReset.Awake))]
        private static void CreateStage(vgReset __instance)
        {
            resetObject = __instance;
            // All objects are eventually destroyed, unless they are children of this "reset object".
            // So we make the reset object the parent of the VR Stage, to make sure we keep it alive.
            VrStage.Create(__instance.transform);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgDestroyAllGameObjects), "OnEnter")]
        private static void PreventStageDestructionStart(object __instance)
        {
            originalParent = StageInstance.transform.parent.parent;
            StageInstance.transform.parent.SetParent(resetObject.transform);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgDestroyAllGameObjects), "OnEnter")]
        private static void PreventStageDestructionEnd()
        {
            StageInstance.transform.parent.SetParent(originalParent);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(vgPlayerController), nameof(vgPlayerController.Start))]
        public static void SetUpStagePlayer(vgPlayerController __instance)
        {
            // Getting camera manually because cameraController.camera isn't set up yet.
            var camera = __instance.cameraController.GetComponentInChildren<Camera>();

            StageInstance.SetUp(camera, __instance);
        }
    }
}