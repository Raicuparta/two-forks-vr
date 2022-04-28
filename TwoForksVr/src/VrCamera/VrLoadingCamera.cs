using TwoForksVr.Helpers;
using UnityEngine;

namespace TwoForksVr.VrCamera;

public class VrLoadingCamera : MonoBehaviour
{
    private vgLoadingCamera loadingCamera;
    private Camera vrCamera;

    public static void Create(vgLoadingCamera loadingCamera)
    {
        var gameObject = new GameObject("VrLoadingCamera");
        var instance = gameObject.AddComponent<VrLoadingCamera>();
        instance.loadingCamera = loadingCamera;
    }

    private void Start()
    {
        SetUpVrCamera();
        SetUpOnlyLoadOnce();
        SetUpParentCanvas();
        DisableOriginalCamera();
        SetUpLoadingCanvas();
    }

    private void LateUpdate()
    {
        if (!loadingCamera)
        {
            Logs.WriteError("VrLoadingCamera missing vgLoadingCamera property");
            Destroy(gameObject);
            return;
        }

        vrCamera.enabled = loadingCamera.isActiveAndEnabled;
    }

    private void DisableOriginalCamera()
    {
        var camera = loadingCamera.GetComponent<Camera>();
        camera.enabled = false;
    }

    private void SetUpVrCamera()
    {
        vrCamera = gameObject.AddComponent<Camera>();
        vrCamera.cullingMask = LayerMask.GetMask("UI");
        vrCamera.clearFlags = CameraClearFlags.SolidColor;
        vrCamera.backgroundColor = Color.black;
    }

    private void SetUpOnlyLoadOnce()
    {
        var onlyLoadOnce = gameObject.AddComponent<vgOnlyLoadOnce>();
        onlyLoadOnce.dontDestroyOnLoad = true;
        onlyLoadOnce.dontDestroyOnReset = true;
    }

    private void SetUpParentCanvas()
    {
        var parentCanvas = loadingCamera.transform.parent.parent.gameObject.AddComponent<Canvas>();
        parentCanvas.worldCamera = vrCamera;
        parentCanvas.renderMode = RenderMode.ScreenSpaceCamera;
    }

    private void SetUpLoadingCanvas()
    {
        var canvas = loadingCamera.transform.parent.GetComponent<Canvas>();
        canvas.transform.localPosition = Vector3.zero;
        canvas.transform.localScale = Vector3.one * 0.5f;

        // Move loading spinner from corner to center.
        var loadSpinner = canvas.transform.Find("LoadSpinner/UI_LoadSpinner/");
        var loadSpinnerPosition = loadSpinner.localPosition;
        loadSpinner.localPosition = new Vector3(0, -150, loadSpinnerPosition.z);
        loadSpinner.localScale = Vector3.one * 1.5f;
    }
}