using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ToolPicker : MonoBehaviour {
	private const float circleRadius = 0.25f;
	private const float minSquareDistance = 0.03f;

	private ToolPickerItem[] tools;
	private SteamVR_Action_Boolean input = SteamVR_Actions.default_ToolPicker;
	private ToolPickerItem selectedTool;
	private string[] toolNames = new[]
	{
		"Radio",
		"Map",
		"Compass",
	};

	public Transform ParentWhileActive;
	public Transform ParentWhileInactive;
	public Transform ToolsContainer;
	public Transform Hand;

	private void Start()
    {
		CreateItems();
		SetUpToolsList();
	}

	private void CreateItems()
    {
		foreach (var toolName in toolNames)
        {
			var toolWrapper = new GameObject(toolName).transform;
			toolWrapper.SetParent(ToolsContainer, false);
			var tool = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
			tool.localScale = Vector3.one * 0.1f;
			tool.SetParent(toolWrapper);
			tool.name = toolName;
			tool.gameObject.AddComponent<ToolPickerItem>();
        }
    }

	private void SetUpToolsList()
    {
		tools = ToolsContainer.GetComponentsInChildren<ToolPickerItem>();
		for (var i = 0; i < tools.Length; i++)
		{
			var tool = tools[i];
			float angle = i * Mathf.PI * 2f / tools.Length;
			tool.transform.localPosition = new Vector3(Mathf.Cos(angle) * circleRadius, Mathf.Sin(angle) * circleRadius, 0);
		}
	}

	private float GetSquareDistance(Vector3 pointA, Vector3 pointB)
    {
		return (pointA - pointB).sqrMagnitude;
    }

	private float GetDistanceToHand(Transform compareTransform)
    {
		return GetSquareDistance(compareTransform.position, Hand.position);
	}

	private void OpenToolPicker()
	{
		ToolsContainer.gameObject.SetActive(true);
		ToolsContainer.SetParent(ParentWhileActive);
		ToolsContainer.LookAt(Camera.main.transform);
	}

	private void CloseToolPicker()
	{
		ToolsContainer.gameObject.SetActive(false);
		ToolsContainer.SetParent(ParentWhileInactive);
		ToolsContainer.localPosition = Vector3.zero;
		ToolsContainer.localRotation = Quaternion.identity;

		if (selectedTool)
		{
			selectedTool.PickTool();
		}
	}

	private void UpdateSelectedTool()
    {
		ToolPickerItem nextSelectedTool = null;
		var selectedToolDistance = Mathf.Infinity;

		for (int i = 0; i < tools.Length; i++)
        {
			var tool = tools[i];
			var distance = GetDistanceToHand(tool.transform);
			if (distance < minSquareDistance && distance < selectedToolDistance)
            {
				nextSelectedTool = tool;
				selectedToolDistance = distance;
			}
        }
		if (nextSelectedTool == selectedTool)
        {
			return;
        }
		if (selectedTool)
        {
			selectedTool.Deselect();
			selectedTool = null;
        }
		if (nextSelectedTool)
        {
			selectedTool = nextSelectedTool;
			nextSelectedTool.Select();
        }
	}

	private void Update () {
		if (input.state)
        {
			UpdateSelectedTool();
		}
		if (input.stateDown)
        {
			OpenToolPicker();
		}
		if (input.stateUp)
		{
			CloseToolPicker();
		}
	}
}
