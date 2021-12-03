using TwoForksVr.Assets;
using TwoForksVr.Stage;
using TwoForksVr.UI;
using UnityEngine;

namespace TwoForksVr
{
    public class VrSettingsMenu: MonoBehaviour
    {
        public static void Create(VrStage stage)
        {
            var instance = Instantiate(VrAssetLoader.VrSettingsMenuPrefab, stage.transform, false).AddComponent<VrSettingsMenu>();
            instance.gameObject.AddComponent<AttachToCamera>();
        }
    }
}