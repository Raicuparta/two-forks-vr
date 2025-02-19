﻿using System.Collections.Generic;
using TwoForksVr.Assets;
using TwoForksVr.Helpers;
using TwoForksVr.Limbs;
using TwoForksVr.VrInput.ActionInputs;
using UnityEngine;

namespace TwoForksVr.Tools;

public class ToolPicker : MonoBehaviour
{
    private const float minSquareDistance = 0.03f;

    private readonly IActionInput input = ActionInputDefinitions.ToolPicker;
    private Transform hand;
    private ToolPickerItem hoveredTool;
    private ToolPickerItem selectedTool;
    private List<ToolPickerItem> tools;
    private Transform toolsContainer;
    public bool IsOpen => toolsContainer && toolsContainer.gameObject.activeSelf;

    public static ToolPicker Create(VrLimbManager limbManager, VrHand dominantHand)
    {
        var instance = Instantiate(VrAssetLoader.ToolPickerPrefab).AddComponent<ToolPicker>();
        instance.transform.SetParent(limbManager.transform, false);
        instance.toolsContainer = instance.transform.Find("Tools");
        instance.hand = dominantHand.transform;

        instance.tools = new List<ToolPickerItem>();
        foreach (Transform child in instance.toolsContainer) instance.tools.Add(ToolPickerItem.Create(child));

        return instance;
    }

    private void Start()
    {
        CloseToolPicker();
    }

    private void Update()
    {
        if (input.ButtonValue) UpdateSelectedTool();
        if (input.ButtonDown) OpenToolPicker();
        if (input.ButtonUp) CloseToolPicker();
    }

    private void SetUpTools()
    {
        var activeCount = 0;
        foreach (var tool in tools)
        {
            var isAllowed = tool.IsAllowed();
            tool.gameObject.SetActive(isAllowed);
            if (isAllowed) activeCount++;
        }

        var index = 0;
        foreach (var tool in tools)
        {
            if (!tool.gameObject.activeSelf) continue;
            tool.SetUp(index, activeCount);
            index++;
        }
    }

    private void SelectCurrentlyHoveredTool()
    {
        if (!hoveredTool) return;

        hoveredTool.Select();
        hoveredTool.EndHover();
        selectedTool = hoveredTool;
        hoveredTool = null;
    }

    private void DeselectCurrentlySelectedTool()
    {
        if (!selectedTool) return;

        selectedTool.Deselect();
        selectedTool = null;
    }

    private void OpenToolPicker()
    {
        if (toolsContainer.gameObject.activeSelf || !Camera.main) return;

        toolsContainer.gameObject.SetActive(true);
        toolsContainer.position = hand.position;
        toolsContainer.LookAt(Camera.main.transform);
        DeselectCurrentlySelectedTool();
        SetUpTools();
    }

    private void CloseToolPicker()
    {
        if (!toolsContainer.gameObject.activeSelf) return;

        toolsContainer.gameObject.SetActive(false);
        toolsContainer.SetParent(transform);
        toolsContainer.localPosition = Vector3.zero;
        toolsContainer.localRotation = Quaternion.identity;
        SelectCurrentlyHoveredTool();
    }

    private void UpdateSelectedTool()
    {
        ToolPickerItem nextSelectedTool = null;
        var selectedToolDistance = Mathf.Infinity;

        foreach (var tool in tools)
        {
            var distance = MathHelper.GetSquareDistance(tool.transform, hand);
            if (!(distance < minSquareDistance) || !(distance < selectedToolDistance)) continue;
            nextSelectedTool = tool;
            selectedToolDistance = distance;
        }

        if (nextSelectedTool == hoveredTool) return;
        if (hoveredTool)
        {
            hoveredTool.EndHover();
            hoveredTool = null;
        }

        if (nextSelectedTool)
        {
            hoveredTool = nextSelectedTool;
            nextSelectedTool.StartHover();
        }
    }
}