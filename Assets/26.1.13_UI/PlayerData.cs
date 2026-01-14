using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerJobData : ScriptableObject
{
    public PlayerData defaultData;
    public Sprite sprite;
}
[System.Serializable]
public class PlayerData
{
    public float hp;
    public int atk;
    public event Action OnHPChange;
    public float Hp
    {
        get { return hp; }
        set
        {
            hp = value;
            OnHPChange?.Invoke();
        }
    }
    public void Set(PlayerData data)
    {
        Hp = data.hp;
        atk = data.atk;
    }
}
