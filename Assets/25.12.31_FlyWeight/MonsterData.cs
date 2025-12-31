using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlyWeight
{
    [CreateAssetMenu]
    public class MonsterData : ScriptableObject
    {
        public int maxHp;
        public Sprite monsterImage;
        public string monsterName;
        public List<string> dropList;
    }

}
