using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlyWeight
{
    //과제2: 몬스터 있고, 몬스터 속성(멤버변수)존재
    //공유되어야 하는 데이터(MaxHp)와
    //공유되면 안되는 객체만의 데이터(hp)
    //2개를 분리해서 SO로 빼기
    public class MonsterSO : MonoBehaviour
    {
        public int hp = 100;
        public MonsterData data;
        public void Die()
        {
            DropItem();
            Destroy(gameObject);
        }

        public void DropItem()
        {
            int randIndex = UnityEngine.Random.Range(0, data.dropList.Count);
            Debug.Log(data.dropList[randIndex]);
        }
        void Start()
        {
            hp = 100;
        }

        // Update is called once per frame
        void Update()
        {
            if (hp <= 0) Die();

        }
    }
}
