using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string toolTip;
    public int price;
    public Sprite sprite;
}

[System.Serializable]
public class Item
{
    [SerializeField]
    public ItemData data;

    public Item(ItemData data)
    {
        this.data = data;
    }
    public virtual void Use(Player target)
    {
        Debug.Log($"{data.itemName} 아이템 사용");
    }
}

public class ItemComponent : MonoBehaviour
{
    public ItemData data;
    public Item item;
    // Start is called before the first frame update
    void Start()
    {
        item = new Item(data);
    }
}
