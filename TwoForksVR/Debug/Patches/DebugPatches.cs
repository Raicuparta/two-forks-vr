using HarmonyLib;

namespace TwoForksVR.Debug.Patches
{
    [HarmonyPatch(typeof(UnityExplorer.UI.UIManager), "InitUI")]
    public class HideUnityExplorerOnStartup
    {
        // I couldn't get the BepInEx version of UnityExplorer to work.
        // So I'm using the standalone version, but that one was resetting the settings on startup.
        // So I'm patching the setting here, since without this I can't use the game menus in VR.
        public static void Postfix()
        {
            UnityExplorer.UI.UIManager.ShowMenu = false;
        }
    }
}
