using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class Inventory
{
    public Player owner;
    public List<Item> items;
    public Inventory(Player owner)
    {
        this.owner = owner;
        items= new List<Item>();
    }
    public void AddItem(Item item)
    {
        items.Add(item);
        OnRefreshInven?.Invoke();
    }
    public void RemoveItem(int index)
    {
        items.RemoveAt(index);
        OnRefreshInven?.Invoke();
    }
    public void UseItem(int index)
    {
        if(index < 0 || index >= items.Count) 
            return;

        items[index].Use(owner);
        RemoveItem(index);
    }
    public event Action OnRefreshInven;
}

public class Player : MonoBehaviour
{
    public PlayerData data;

    [SerializeField]
    private List<PlayerJobData> jobDatas;
    public PlayerJobData curJob;
    public Inventory inven;

    public event Action OnChangeJob;

    void Start()
    {
        inven = new Inventory(this);
        SetJob(0);
    }

    public void SetJob(int index)
    {
        curJob = jobDatas[index];
        data.Set(curJob.defaultData);
        OnChangeJob?.Invoke();
    }

    public void Hit()
    {
        data.Hp -= 10;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Hit();
        }
       
        if(Input.GetKey(KeyCode.Alpha1))
        {
            SetJob(0);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            SetJob(1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemComponent ic = other.GetComponent<ItemComponent>();
        if(ic != null)
        {
            inven.AddItem(ic.item);
            Destroy(other.gameObject);
        }
    }
}
