using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    public List<SlotView> slotViewList;

    private void Start()
    {
        for(int i=0;i<slotViewList.Count;i++)
        {
            slotViewList[i].slotNum = i;
        }
    }

    public void UpdateUI(List<Item> items)
    {
        for(int i=0;i< items.Count;i++)
        {
            slotViewList[i].UpdateUI(items[i].data.sprite);
        }
        for(int i=items.Count;i<slotViewList.Count;i++)
        {
            slotViewList[i].UpdateUI(null);
        }
    }
}
