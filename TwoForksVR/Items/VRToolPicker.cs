using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace Raicuparta.TwoForksVR
{
	public class VRToolPicker : MonoBehaviour
	{
		private const float circleRadius = 0.25f;
		private const float minSquareDistance = 0.03f;
		private readonly string[] toolNames = new[]
		{
		"Radio",
		"Map",
		"Compass",
		"Backpack",
		"Something else",
	};

		private Transform toolsContainer;
		private List<VRToolPickerItem> toolPickerItems = new List<VRToolPickerItem>();
		private SteamVR_Action_Boolean input = SteamVR_Actions.default_ToolPicker;
		private VRToolPickerItem selectedItem;

		public Transform ParentWhileActive;
		public Transform Hand;

		private void Awake()
		{
			SetUpToolsContainer();
			SetUpItems();
		}

		private void SetUpToolsContainer()
        {
			toolsContainer = new GameObject("VRToolsContainer").transform;
			toolsContainer.SetParent(transform, false);
        }

		private void SetUpItems()
		{
			for (var i = 0; i < toolNames.Length; i++)
			{
				var toolWrapper = new GameObject(toolNames[i]).transform;
				toolWrapper.SetParent(toolsContainer, false);
				toolWrapper.transform.localPosition = MathHelper.PositionAroundCircle(i, toolNames.Length, circleRadius);
				
				var item = toolWrapper.gameObject.AddComponent<VRToolPickerItem>();
				toolPickerItems.Add(item);

				var tool = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
				tool.name = toolNames[i];
				tool.SetParent(toolWrapper, false);
				tool.localScale = Vector3.one * 0.1f;
				tool.GetComponent<Collider>().isTrigger = true;
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
			toolsContainer.gameObject.SetActive(true);
			toolsContainer.SetParent(ParentWhileActive);
			toolsContainer.LookAt(Camera.main.transform);
		}

		private void CloseToolPicker()
		{
			toolsContainer.gameObject.SetActive(false);
			toolsContainer.SetParent(transform);
			toolsContainer.localPosition = Vector3.zero;
			toolsContainer.localRotation = Quaternion.identity;

			if (selectedItem)
			{
				selectedItem.PickTool();
			}
		}

		private void UpdateSelectedItem()
		{
			VRToolPickerItem nextSelectedItem = null;
			var seeltedItemDistance = Mathf.Infinity;

			foreach (var toolPickerItem in toolPickerItems)
			{
				var distance = GetDistanceToHand(toolPickerItem.transform);
				if (distance < minSquareDistance && distance < seeltedItemDistance)
				{
					nextSelectedItem = toolPickerItem;
					seeltedItemDistance = distance;
				}
			}
			if (nextSelectedItem == selectedItem)
			{
				return;
			}
			if (selectedItem)
			{
				selectedItem.Deselect();
				selectedItem = null;
			}
			if (nextSelectedItem)
			{
				selectedItem = nextSelectedItem;
				nextSelectedItem.Select();
			}
		}

		private void Update()
		{
			if (input.state)
			{
				UpdateSelectedItem();
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
}