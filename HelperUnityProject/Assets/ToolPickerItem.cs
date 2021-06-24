using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPickerItem : MonoBehaviour
{
    public ToolPicker.VRToolItem ItemType;

    private bool isHovered = false;

    public void StartHover()
    {
        if (isHovered)
        {
            return;
        }

        isHovered = true;
        transform.localScale *= 1.5f;
    }

    public void EndHover()
    {
        if (!isHovered)
        {
            return;
        }

        isHovered = false;
        transform.localScale = transform.localScale / 1.5f;
    }
}
