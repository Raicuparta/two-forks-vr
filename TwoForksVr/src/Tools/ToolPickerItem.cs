using System;
using TwoForksVr.Tools.ToolPickerActions;
using UnityEngine;

namespace TwoForksVr.Tools
{
    public class ToolPickerItem : MonoBehaviour
    {
        private const float circleRadius = 0.25f;
        private bool isHovered;
        private VrToolItem itemType;
        private ToolPickerAction toolPickerAction;

        public static ToolPickerItem Create(Transform parent, int index)
        {
            var transform = parent.GetChild(index);
            var instance = transform.gameObject.AddComponent<ToolPickerItem>();
            instance.itemType = (VrToolItem) Enum.Parse(typeof(VrToolItem), transform.name);

            var angle = index * Mathf.PI * 2f / parent.childCount;
            instance.transform.localPosition =
                new Vector3(Mathf.Cos(angle) * circleRadius, Mathf.Sin(angle) * circleRadius, 0);

            // TODO separate into individual classes for each item?
            instance.toolPickerAction = ToolPickerAction.GetToolPickerAction(instance.itemType);

            instance.SetUpIcon();

            return instance;
        }

        private void SetUpIcon()
        {
            var icon = transform.Find("Icon").GetComponent<SpriteRenderer>();
            icon.material.shader = Shader.Find("Sprites/Default");
        }

        public void StartHover()
        {
            if (isHovered) return;

            isHovered = true;
            transform.localScale *= 1.5f;
        }

        public void EndHover()
        {
            if (!isHovered) return;

            isHovered = false;
            transform.localScale /= 1.5f;
        }

        public void Select()
        {
            toolPickerAction.Select();
        }

        public void Deselect()
        {
            toolPickerAction.Deselect();
        }
    }
}