using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlyWeight
{
    public abstract class DropTable
    {
        public List<string> items;
        public DropTable()
        {
            items = new List<string>();
            items.Add("가죽");
            Init();
        }
        //템플릿 메서드 패턴
        public abstract void Init();
    }
    public class OrcTable : DropTable
    {
        public override void Init()
        {
            //팩토리 메서트 패턴 - 부모 객체 생성책임을 자식에게 부여
            items.Add("몽둥이");
            items.Add("오크 고기");
            items.Add("오크 손가락");
        }
    }
    public class DragonTable : DropTable
    {
        public override void Init()
        {
            items.Add("용린");
            items.Add("용옥");
        }
    }
    //심플팩토리
    public static class DropTableFactory
    {
        private static Dictionary<string, DropTable> dropDic = new Dictionary<string, DropTable>();
        public static DropTable GetTable(string name)
        {
            //플라이웨이트 패턴 -> SO의 구현
            //이미 있으면 갖다줌
            if (dropDic.ContainsKey(name))
            {
                return dropDic[name];
            }
            DropTable newTable = null;
            //없으면 만듬
            if (name == "Orc")
            {
                //심플팩토리
                newTable = new OrcTable();
            }
            else if (name == "Dragon")
            {
                newTable = new DragonTable();
            }

            dropDic.Add(name, newTable);
            return newTable;
        }

    }
    public class MonsterFlyWeight : MonoBehaviour
    {
        public int hp = 100;
        public string monsterName;
        public void Die()
        {
            DropItem();
            Destroy(gameObject);
        }

        public void DropItem()
        {
            DropTable dropTable = DropTableFactory.GetTable(monsterName);
            int randIndex = UnityEngine.Random.Range(0, dropTable.items.Count);
            Debug.Log(dropTable.items[randIndex]);
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
