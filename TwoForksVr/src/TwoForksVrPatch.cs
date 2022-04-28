using TwoForksVr.Stage;

namespace TwoForksVr;

public abstract class TwoForksVrPatch
{
    protected static VrStage StageInstance;

    public static void SetStage(VrStage vrStage)
    {
        StageInstance = vrStage;
    }
}