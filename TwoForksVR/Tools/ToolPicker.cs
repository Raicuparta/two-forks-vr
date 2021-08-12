using System.Linq;
using TwoForksVR.Assets;
using TwoForksVR.Helpers;
using UnityEngine;
using Valve.VR;

namespace TwoForksVR.Tools
{
    public class ToolPicker : MonoBehaviour
    {
        private const float minSquareDistance = 0.03f;

        private readonly SteamVR_Action_Boolean input = SteamVR_Actions.default_ToolPicker;
        private ToolPickerItem hoveredTool;
        private Transform leftHand;
        private Transform rightHand;
        private ToolPickerItem selectedTool;
        private ToolPickerItem[] tools;
        private Transform toolsContainer;

        private void Start()
        {
            CloseToolPicker();
        }

        private void Update()
        {
            if (input.GetState(SteamVR_Input_Sources.RightHand)) UpdateSelectedTool(rightHand);
            if (input.GetState(SteamVR_Input_Sources.LeftHand)) UpdateSelectedTool(leftHand);
            if (input.GetStateDown(SteamVR_Input_Sources.RightHand)) OpenToolPicker(rightHand);
            if (input.GetStateDown(SteamVR_Input_Sources.LeftHand)) OpenToolPicker(leftHand);
            if (input.stateUp) CloseToolPicker();
        }

        public static ToolPicker Create(Transform parent, Transform leftHand, Transform rightHand)
        {
            var instance = Instantiate(VRAssetLoader.ToolPicker).AddComponent<ToolPicker>();
            instance.transform.SetParent(parent, false);
            instance.toolsContainer = instance.transform.Find("Tools");
            instance.rightHand = rightHand;
            instance.leftHand = leftHand;

            instance.tools = instance.toolsContainer.Cast<Transform>().Select(
                (child, index) => ToolPickerItem.Create(
                    instance.toolsContainer,
                    index
                )
            ).ToArray();

            return instance;
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
            if (toolsContainer.gameObject.activeSelf || !Camera.main) return;

            toolsContainer.gameObject.SetActive(true);
            toolsContainer.position = hand.position;
            toolsContainer.LookAt(Camera.main.transform);
            DeselectCurrentlySelectedTool();
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

        private void UpdateSelectedTool(Transform hand)
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
}