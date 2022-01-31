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

        public static ToolPickerItem Create(Transform transform)
        {
            var instance = transform.gameObject.AddComponent<ToolPickerItem>();
            instance.itemType = (VrToolItem) Enum.Parse(typeof(VrToolItem), transform.name);


            instance.toolPickerAction = ToolPickerAction.GetToolPickerAction(instance.itemType);

            instance.SetUpIcon();

            return instance;
        }

        public void SetUp(int index, int totalCount)
        {
            var angle = index * Mathf.PI * 2f / totalCount;
            transform.localPosition = new Vector3(Mathf.Cos(angle) * circleRadius, Mathf.Sin(angle) * circleRadius, 0);
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
            toolPickerAction.Equip();
        }

        public void Deselect()
        {
            toolPickerAction.Unequip();
        }

        public bool IsAllowed()
        {
            return toolPickerAction.CanEquip();
        }
    }
}