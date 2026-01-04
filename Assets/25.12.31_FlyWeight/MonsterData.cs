using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class MonsterData : ScriptableObject
{
    public int maxHp;
    public Sprite monsterImage;
    public string monsterName;
    public List<GameObject> dropList;
}

