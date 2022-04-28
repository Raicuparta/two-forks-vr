using TwoForksVr.Helpers;

namespace TwoForksVr.UI;

public class StaticUi : AttachedUi
{
    private void Awake()
    {
        MaterialHelper.MakeGraphicChildrenDrawOnTop(gameObject);
    }
}