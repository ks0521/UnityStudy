using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

namespace Builder
{
    //빌더 패턴 : 생성부(객체의 생성)와 표현부(값을 넣기)를 분리
    public class Monster
    {
        public string name;
        public int hp;
        public int atk;

    }

    public class MonsterBuilder
    {
        private Monster mon;
        public MonsterBuilder()
        {
            mon = new Monster();
        }
        public MonsterBuilder SetName(string name)
        {
            mon.name = name;
            return this;
        }
        public MonsterBuilder SetHp(int hp)
        {
            mon.hp = hp;
            return this;
        }
        public MonsterBuilder setAtk(int atk)
        {
            mon.atk = atk;
            return this;
        }
        public Monster Build()
        {
            return mon;
        }
    }
    public class BuilderTest : MonoBehaviour
    {
        MonsterBuilder builder;
        Monster mon;
        private void Start()
        {
            mon = builder.SetHp(100).SetName("슬라임").setAtk(10).Build();
            StringBuilder sb = new StringBuilder();
            sb.Append("A").Append("B").Append("C").ToString();
        }
    }
}