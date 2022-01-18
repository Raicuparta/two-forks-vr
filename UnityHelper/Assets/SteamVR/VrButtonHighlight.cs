using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

// ReSharper disable MemberCanBePrivate.Global

public class VrButtonHighlight : MonoBehaviour
{
    public Material controllerMaterial;
    public Color flashColor = new Color(1.0f, 0.557f, 0.0f);
    private readonly Dictionary<string, Transform> componentTransformMap = new Dictionary<string, Transform>();
    private readonly List<MeshRenderer> flashingRenderers = new List<MeshRenderer>();
    private readonly List<MeshRenderer> renderers = new List<MeshRenderer>();
    private Dictionary<ISteamVR_Action_In_Source, ActionHintInfo> actionHintInfos;
    private int colorID;
    private SteamVR_Input_Sources inputSource;
    private Transform player;
    private SteamVR_RenderModel renderModel;
    private SteamVR_Events.Action renderModelLoadedAction;
    private float startTime;
    private Transform textHintParent;
    private float tickCount;

    public Material UsingMaterial
    {
        get { return controllerMaterial; }
    }

    public bool Initialized { get; private set; }

    private void Awake()
    {
        renderModelLoadedAction = SteamVR_Events.RenderModelLoadedAction(OnRenderModelLoaded);
        renderModel = GetComponent<SteamVR_RenderModel>();
        colorID = Shader.PropertyToID("_Color");
    }

    private void Update()
    {
        if (!renderModel || !renderModel.gameObject.activeInHierarchy || flashingRenderers.Count <= 0) return;
        var baseColor = UsingMaterial.GetColor(colorID);

        var flash = (Time.realtimeSinceStartup - startTime) * Mathf.PI * 2.0f;
        flash = Mathf.Cos(flash);
        flash = Util.RemapNumberClamped(flash, -1.0f, 1.0f, 0.0f, 1.0f);

        var ticks = Time.realtimeSinceStartup - startTime;
        if (ticks - tickCount > 1.0f) tickCount += 1.0f;

        foreach (var r in flashingRenderers) r.material.SetColor(colorID, Color.Lerp(baseColor, flashColor, flash));
    }


    private void OnEnable()
    {
        renderModelLoadedAction.enabled = true;
    }


    private void OnDisable()
    {
        renderModelLoadedAction.enabled = false;
        Clear();
    }


    public void SetInputSource(SteamVR_Input_Sources newInputSource)
    {
        inputSource = newInputSource;
        if (renderModel != null)
            renderModel.SetInputSource(newInputSource);
    }

    private void OnRenderModelLoaded(SteamVR_RenderModel loadedRenderModel, bool succeess)
    {
        //Only initialize when the render model for the controller hints has been loaded
        if (loadedRenderModel != renderModel) return;

        if (Initialized)
        {
            Destroy(textHintParent.gameObject);
            componentTransformMap.Clear();
            flashingRenderers.Clear();
        }

        loadedRenderModel.SetMeshRendererState(false);

        StartCoroutine(DoInitialize(loadedRenderModel));
    }

    private IEnumerator DoInitialize(SteamVR_RenderModel initRenderModel)
    {
        while (initRenderModel.initializedAttachPoints == false)
            yield return null;

        textHintParent = new GameObject("Text Hints").transform;
        textHintParent.SetParent(transform);
        textHintParent.localPosition = Vector3.zero;
        textHintParent.localRotation = Quaternion.identity;
        textHintParent.localScale = Vector3.one;

        //Get the button mask for each component of the render model

        var renderModels = OpenVR.RenderModels;
        if (renderModels != null)
            for (var childIndex = 0; childIndex < initRenderModel.transform.childCount; childIndex++)
            {
                var child = initRenderModel.transform.GetChild(childIndex);

                if (!componentTransformMap.ContainsKey(child.name)) componentTransformMap.Add(child.name, child);
            }

        actionHintInfos = new Dictionary<ISteamVR_Action_In_Source, ActionHintInfo>();

        foreach (var action in SteamVR_Input.actionsNonPoseNonSkeletonIn)
            if (action.GetActive(inputSource))
                CreateAndAddButtonInfo(action, inputSource);

        Initialized = true;

        //Set the controller hints render model to not active
        initRenderModel.SetMeshRendererState(true);
        initRenderModel.gameObject.SetActive(false);
    }


    private void CreateAndAddButtonInfo(ISteamVR_Action_In action, SteamVR_Input_Sources buttonInputSource)
    {
        Transform buttonTransform = null;
        var buttonRenderers = new List<MeshRenderer>();
        var actionComponentName = action.GetRenderModelComponentName(buttonInputSource);

        if (componentTransformMap.ContainsKey(actionComponentName))
        {
            var componentTransform = componentTransformMap[actionComponentName];
            buttonTransform = componentTransform;
            buttonRenderers.AddRange(componentTransform.GetComponentsInChildren<MeshRenderer>());
        }

        if (!buttonTransform)
        {
            Debug.Log("Couldn't find buttonTransform for " + action.GetShortName());
            return;
        }

        var hintInfo = new ActionHintInfo();
        actionHintInfos.Add(action, hintInfo);
        hintInfo.Renderers = buttonRenderers;
    }

    public void ShowButtonHint(params ISteamVR_Action_In_Source[] actions)
    {
        renderModel.gameObject.SetActive(true);

        renderModel.GetComponentsInChildren(renderers);
        foreach (var childRenderer in renderers)
        {
            var mainTexture = childRenderer.material.mainTexture;
            childRenderer.sharedMaterial = UsingMaterial;
            childRenderer.material.mainTexture = mainTexture;

            // This is to poke unity into setting the correct render queue for the model
            childRenderer.material.renderQueue = UsingMaterial.shader.renderQueue;
        }

        foreach (var t in actions)
            if (actionHintInfos.ContainsKey(t))
            {
                var hintInfo = actionHintInfos[t];
                foreach (var hitInfoRenderer in hintInfo.Renderers)
                    if (!flashingRenderers.Contains(hitInfoRenderer))
                        flashingRenderers.Add(hitInfoRenderer);
            }

        startTime = Time.realtimeSinceStartup;
        tickCount = 0.0f;
    }


    public void HideAllButtonHints()
    {
        Clear();

        if (renderModel != null && renderModel.gameObject != null)
            renderModel.gameObject.SetActive(false);
    }


    public void HideButtonHint(params ISteamVR_Action_In_Source[] actions)
    {
        var baseColor = UsingMaterial.GetColor(colorID);
        foreach (var action in actions)
            if (actionHintInfos.ContainsKey(action))
            {
                var hintInfo = actionHintInfos[action];
                foreach (var hitInfoRenderer in hintInfo.Renderers)
                {
                    hitInfoRenderer.material.color = baseColor;
                    flashingRenderers.Remove(hitInfoRenderer);
                }
            }

        if (flashingRenderers.Count == 0) renderModel.gameObject.SetActive(false);
    }

    public bool IsButtonHintActive(ISteamVR_Action_In_Source action)
    {
        if (!actionHintInfos.ContainsKey(action)) return false;
        var hintInfo = actionHintInfos[action];
        foreach (var buttonRenderer in hintInfo.Renderers)
            if (flashingRenderers.Contains(buttonRenderer))
                return true;

        return false;
    }

    private void Clear()
    {
        renderers.Clear();
        flashingRenderers.Clear();
    }

    //Info for each of the buttons
    private class ActionHintInfo
    {
        public List<MeshRenderer> Renderers;
    }
}