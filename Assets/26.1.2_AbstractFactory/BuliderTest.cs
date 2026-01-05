using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

namespace Builder
{
    [System.Serializable]
    public class Monster
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private int hp;
        [SerializeField]
        private int atk;

        public Monster(string name, int hp, int atk)
        {
            this.name = name;
            this.hp = hp;
            this.atk = atk;
        }
    }
    //빌더 패턴 : 생성부(객체의 생성)와 표현부(값을 넣기)를 분리

    public class MonsterBuilder
    {
        private string name;
        private int hp;
        private int atk;
        GameObject prefeb;
        public MonsterBuilder(GameObject prefeb)
        {
            this.prefeb = prefeb;
        }
        public MonsterBuilder SetName(string name)
        {
            this.name = name;
            return this;
        }
        public MonsterBuilder SetHp(int hp)
        {
            this.hp = hp;
            return this;
        }
        public MonsterBuilder setAtk(int atk)
        {
            this.atk = atk;
            return this;
        }
        public void Build()
        {
            GameObject newObj = GameObject.Instantiate(prefeb);
            MonsterComponent mc = newObj.GetComponent<MonsterComponent>();
            mc.monData = new Monster(name,hp,atk);
        }
    }
    public class BuliderTest : MonoBehaviour
    {
        MonsterBuilder builder;
        public GameObject monsterPrefeb;

        Monster mon;
        private void Start()
        {
            builder = new MonsterBuilder(monsterPrefeb);
            builder.SetHp(100).SetName("슬라임").setAtk(10);
            StringBuilder sb = new StringBuilder();
            sb.Append("A").Append("B").Append("C").ToString();
            Debug.Log(sb);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                builder.Build();
            }
        }
    }
}
