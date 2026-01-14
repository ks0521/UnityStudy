using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
public class SlotView : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    public int slotNum;
    public Image itemImage;
    public event Action OnClick;
    public event Action OnEnter;
    public event Action OnExit;

    public void UpdateUI(Sprite sprite)
    {
        itemImage.gameObject.SetActive(sprite != null);
        itemImage.sprite = sprite;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit?.Invoke();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
}
