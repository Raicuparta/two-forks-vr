using Harmony;
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
		private static VRToolPicker instance;

		private Transform toolsContainer;
		private List<VRToolPickerItem> toolPickerItems = new List<VRToolPickerItem>();
		private SteamVR_Action_Boolean input = SteamVR_Actions.default_ToolPicker;
		private VRToolPickerItem selectedItem;

		public Transform ParentWhileActive;
		public Transform Hand;

		private void Awake()
		{
			instance = this;
			SetUpToolsContainer();
			//SetUpItems();

			AddRadio();
			AddRadio();
			AddRadio();
			AddRadio();
			AdjustItemPositions();
		}

		private void SetUpToolsContainer()
        {
			toolsContainer = new GameObject("VRToolsContainer").transform;
			toolsContainer.SetParent(transform, false);
			CloseToolPicker();
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

		private VRToolPickerItem AddToolItem(Transform toolItem)
        {
			if (!toolsContainer)
            {
				SetUpToolsContainer();
			}

			var toolWrapper = new GameObject($"VRToolWrapper-{toolItem.name}").transform;
			toolWrapper.SetParent(toolsContainer, false);
			//toolWrapper.transform.localPosition = Vector3.zero;
			var count = toolPickerItems.Count;
			MelonLogger.Msg("Gonna add tool item " + count);
            //toolWrapper.transform.localPosition = MathHelper.PositionAroundCircle(count, 4, circleRadius);

            var item = toolWrapper.gameObject.AddComponent<VRToolPickerItem>();
			toolPickerItems.Add(item);

			toolItem.SetParent(toolWrapper, false);
			toolItem.localPosition = Vector3.zero;
			toolItem.localRotation = Quaternion.identity;

			return item;
		}

		private void AdjustItemPositions()
        {
			MelonLogger.Msg("##### AdjustItemPositions");
			for (var i = 0; i < toolPickerItems.Count; i++)
			{
				toolPickerItems[i].transform.localPosition = MathHelper.PositionAroundCircle(i, toolPickerItems.Count, circleRadius);
				MelonLogger.Msg("##### AdjustItemPositions " + MathHelper.PositionAroundCircle(i, toolPickerItems.Count, circleRadius));
			}
        }

		private void AddRadio()
        {
			var radioController = FindObjectOfType<vgPlayerRadioControl>();
			if (!radioController)
            {
				return;
            }
			var originalRadio = radioController.radio.transform;
			var originalModel = originalRadio.Find("RadioGeo");
			var clonedRadio = Instantiate(originalModel);

			var renderers = clonedRadio.GetComponentsInChildren<MeshRenderer>(true);
			foreach (var renderer in renderers)
            {
				renderer.enabled = true;
            }
			var toolItem = AddToolItem(clonedRadio);
			toolItem.OnEquipTool = () =>
			{
				radioController.OnRadioUp();
			};
			toolItem.OnUnequipTool = () =>
			{
				radioController.OnRadioDown();
			};
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

		[HarmonyPatch(typeof(vgPlayerRadioControl), "Start")]
		public class PatchRadioStart
		{
			//public static void Postfix(GameObject ___radio)
			//{
			//	instance.AddToolItem(___radio);
			//}
		}
	}
}