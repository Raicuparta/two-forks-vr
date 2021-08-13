using System;
using HarmonyLib;
using HutongGames.PlayMaker.Actions;
using Object = UnityEngine.Object;

namespace TwoForksVR.Stage
{
    [HarmonyPatch]
    public static class StagePatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgReset), "Awake")]
        private static void CreateStage(vgReset __instance)
        {
            TwoForksVRMod.LogWarning("RESET AWAKE");
            // All objects are eventually destroyed, unless they are children of this "reset object".
            VRStage.Create(__instance.transform);
        }
        
                [HarmonyPrefix]
        [HarmonyPatch(typeof(vgOnlyLoadOnce), "Awake")]
        private static void OnlyLoadOnce()
        {
            TwoForksVRMod.LogWarning("OnlyLoadOnce AWAKE");
        }

        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(vgDestroyAllGameObjects), "OnEnter")]
        // private static bool Nonono(vgDestroyAllGameObjects __instance)
        // {
        //     return false;
        // }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Object), "Destroy", typeof(Object))]
        [HarmonyPatch(typeof(Object), "Destroy", typeof(Object), typeof(float))]
        [HarmonyPatch(typeof(Object), "DestroyObject", typeof(Object))]
        [HarmonyPatch(typeof(Object), "DestroyObject", typeof(Object), typeof(float))]
        [HarmonyPatch(typeof(Object), "DestroyImmediate", typeof(Object))]
        [HarmonyPatch(typeof(Object), "DestroyImmediate", typeof(Object), typeof(bool))]
        private static bool DestroyGameObject(Object obj)
        {
            return obj != VRStage.Instance;
        }
    }
}