using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPickerItem : MonoBehaviour
{
    private bool isSelected = false;

	private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("collider " + collider);
    }

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
        transform.SetParent(null);
    }
}
