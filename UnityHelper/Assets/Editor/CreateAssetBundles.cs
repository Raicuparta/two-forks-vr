using UnityEditor;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem("Tools/Create Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "../TwoForksVr/TwoForksVrAssets/AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
                                        BuildAssetBundleOptions.None,
                                        BuildTarget.StandaloneWindows);
    }
}
