﻿using System;
using System.Collections;
using System.Collections.Generic;
using TwoForksVR.Hands;
using UnityEngine;
using Valve.VR;

namespace TwoForksVR.Tools
{
	public class ToolPicker : MonoBehaviour
	{
		public enum VRToolItem
		{
			Radio,
			Map,
			Compass,
			Flashlight,
			DisposableCamera,
		}

		public Transform ParentWhileActive;
		public Transform ParentWhileInactive;
		public Transform ToolsContainer;

		private const float circleRadius = 0.25f;
		private const float minSquareDistance = 0.03f;

		private ToolPickerItem[] tools;
		private SteamVR_Action_Boolean input = SteamVR_Actions.default_ToolPicker;
		private ToolPickerItem hoveredTool;
		private ToolPickerItem selectedTool;

		private void Start()
		{
			SetUpToolsList();
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

		private float GetDistanceToHand(Transform compareTransform, Transform hand)
		{
			return GetSquareDistance(compareTransform.position, hand.position);
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

		private void OpenToolPicker(Transform hand)
		{
			if (ToolsContainer.gameObject.activeSelf) return;

			ToolsContainer.gameObject.SetActive(true);
			ToolsContainer.SetParent(ParentWhileActive);
			ToolsContainer.position = hand.position;
			ToolsContainer.LookAt(Camera.main.transform);
			DeselectCurrentlySelectedTool();
		}

		private void CloseToolPicker()
		{
			if (!ToolsContainer.gameObject.activeSelf) return;

			ToolsContainer.gameObject.SetActive(false);
			ToolsContainer.SetParent(ParentWhileInactive);
			ToolsContainer.localPosition = Vector3.zero;
			ToolsContainer.localRotation = Quaternion.identity;
			SelectCurrentlyHoveredTool();
		}

		private void UpdateSelectedTool(Transform hand)
		{
			ToolPickerItem nextSelectedTool = null;
			var selectedToolDistance = Mathf.Infinity;

			for (int i = 0; i < tools.Length; i++)
			{
				var tool = tools[i];
				var distance = GetDistanceToHand(tool.transform, hand);
				if (distance < minSquareDistance && distance < selectedToolDistance)
				{
					nextSelectedTool = tool;
					selectedToolDistance = distance;
				}
			}
			if (nextSelectedTool == hoveredTool)
			{
				return;
			}
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

		private void Update()
		{
			if (input.GetState(SteamVR_Input_Sources.RightHand))
			{
				UpdateSelectedTool(VRHandsManager.Instance.RightHand);
			}
			if (input.GetState(SteamVR_Input_Sources.LeftHand))
			{
				UpdateSelectedTool(VRHandsManager.Instance.LeftHand);
			}
			if (input.GetStateDown(SteamVR_Input_Sources.RightHand))
			{
				OpenToolPicker(VRHandsManager.Instance.RightHand);
			}
			if (input.GetStateDown(SteamVR_Input_Sources.LeftHand))
			{
				OpenToolPicker(VRHandsManager.Instance.LeftHand);
			}
			if (input.stateUp)
			{
				CloseToolPicker();
			}
		}
	}
}