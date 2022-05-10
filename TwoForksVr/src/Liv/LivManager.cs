using TwoForksVr.Helpers;
using TwoForksVr.Stage;
using UnityEngine;

namespace TwoForksVr.Liv;

public class LivManager : MonoBehaviour
{
    private LIV.SDK.Unity.LIV liv;

    public static LivManager Create(VrStage stage)
    {
        var instance = new GameObject("LivManager").AddComponent<LivManager>();
        instance.transform.SetParent(stage.transform, false);
        return instance;
    }

    public void SetUp(Camera camera)
    {
        gameObject.SetActive(false);
        var existingLiv = gameObject.GetComponent<LIV.SDK.Unity.LIV>();
        if (existingLiv) Destroy(existingLiv);
        liv = gameObject.AddComponent<LIV.SDK.Unity.LIV>();
        liv.HMDCamera = camera;
        liv.stage = transform;
        liv.excludeBehaviours = new[]
        {
            "GUILayer",
            "Animation",
            "AkAudioListener",
            "Recorder",
            "vgDeferredGlobalFog",
            "vgStylisticFog",
            "vgCameraModeEffectsController",
            "vgFullscreenRenderTextureCamera",
            "FadeOverlay"
        };
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!liv || !liv.isActive) return;
        liv.spectatorLayerMask = liv.HMDCamera.cullingMask & ~(1 << (int)GameLayer.VrHands) & ~(1 << (int)GameLayer.PlayerBody);
        var livCamera = liv.render.cameraInstance;
        livCamera.clearFlags = liv.HMDCamera.clearFlags;
        livCamera.backgroundColor = liv.HMDCamera.backgroundColor;
        livCamera.stereoTargetEye = StereoTargetEyeMask.None;
    }
}