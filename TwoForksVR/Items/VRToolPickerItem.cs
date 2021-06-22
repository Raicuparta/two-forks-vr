using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raicuparta.TwoForksVR
{
    public class VRToolPickerItem : MonoBehaviour
    {
        public Action OnEquipTool;
        public Action OnUnequipTool;

        private bool isSelected = false;

        public void Select()
        {
            if (isSelected)
            {
                return;
            }

            isSelected = true;
            transform.localScale = Vector3.one * 1.5f;
        }

        public void Deselect()
        {
            if (!isSelected)
            {
                return;
            }

            isSelected = false;
            transform.localScale = Vector3.one;
        }

        public void PickTool()
        {
            OnEquipTool?.Invoke();
        }

        public void PutToolAway()
        {
            OnUnequipTool?.Invoke();
        }
    }
}