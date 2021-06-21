using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ToolPicker : MonoBehaviour {
	private bool isOpen = false;
	private MeshRenderer orbRenderer;
	private List<ToolPickerItem> tools = new List<ToolPickerItem>();
	private SteamVR_Action_Boolean input = SteamVR_Actions.default_Flashlight;
	private ToolPickerItem selectedTool;

	public Transform ParentWhileActive;
	public Transform ParentWhileInactive;
	public Transform ToolsContainer;
	public Transform Hand;

	private void Awake () {
		orbRenderer = GetComponent<MeshRenderer>();

		foreach (Transform child in ToolsContainer)
        {
			var tool = child.GetComponent<ToolPickerItem>();
			if (tool) tools.Add(tool);
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

	private void UpdateSelectedTool()
    {
		var nextSelectedTool = tools[0];
		var selectedToolDistance = GetDistanceToHand(nextSelectedTool.transform);

		for (int i = 1; i < tools.Count; i++)
        {
			var tool = tools[i];
			var distance = GetDistanceToHand(tool.transform);
			if (distance < selectedToolDistance)
            {
				nextSelectedTool = tool;
				selectedToolDistance = distance;
			}
        }
		if (selectedTool)
        {
			selectedTool.Deselect();
        }
		selectedTool = nextSelectedTool;
		nextSelectedTool.Select();
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
}
