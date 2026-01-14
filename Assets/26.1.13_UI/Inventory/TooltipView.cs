using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TooltipView : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    public void UpdateUI(string toolTip)
    {
        tmp.text = toolTip;
    }
}
