using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPresenter : MonoBehaviour
{
    public InventoryView invenView;
    public TooltipView toolTipView;
    public Player player;

    public void Start()
    {
        player.inven.OnRefreshInven += RefreshInventoryUI;
        
        for(int i=0;i<invenView.slotViewList.Count;i++)
        {
            int curIndex = i;
            invenView.slotViewList[i].OnClick += ()=> {
                player.inven.UseItem(curIndex);
                Debug.Log($"{curIndex} 번째 아이템 사용");
            };
            invenView.slotViewList[i].OnEnter += () =>
            {
                if (invenView.slotViewList[curIndex].itemImage.sprite != null)
                {
                    toolTipView.UpdateUI(player.inven.items[curIndex].data.toolTip);
                    toolTipView.transform.position = invenView.slotViewList[curIndex].transform.position;
                    toolTipView.gameObject.SetActive(true);
                }
            };
            invenView.slotViewList[i].OnExit += () =>
            {
                toolTipView.gameObject.SetActive(false);
            };
        }
    }

    public void RefreshInventoryUI()
    {
        invenView.UpdateUI(player.inven.items);
    }
}
