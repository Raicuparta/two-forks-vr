using TwoForksVr.Assets;
using TwoForksVr.Stage;
using TwoForksVr.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TwoForksVr
{
    public class VrSettingsMenu: MonoBehaviour
    {
        public static void Create(VrStage stage)
        {
            var instance = Instantiate(VrAssetLoader.VrSettingsMenuPrefab, stage.transform, false).AddComponent<VrSettingsMenu>();
            instance.gameObject.AddComponent<AttachToCamera>();

            var canvas = instance.GetComponent<Canvas>();
            canvas.sortingOrder = 1;

            var layoutGroup = instance.transform.Find("LayoutGroup");
            var firstSelectable = layoutGroup.gameObject.GetComponentInChildren<Selectable>();
            firstSelectable.Select();
        }
    }
}