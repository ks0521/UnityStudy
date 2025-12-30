using UnityEngine;
using UnityEngine.UIElements;

public class Day25_11_3 : MonoBehaviour
{
    //객체지향의 3요소
    //1. 캡슐화
    //2. 상속 : 부모가 자식에게 속성과 기능을 물려주는 것
    
    /*            작성한 클래스 도식
     *               Character
     *                /      \
                    User     NPC
                            /   \
                       Monster  Ally
                          |      |
                         Orc  Merchant
     */             

    struct Status
    {
        int hp;
        int atk;
        string name;
        bool isDead;
        public Status(int hp, int atk, string name)
        {
            this.hp = hp;
            this.atk = atk; 
            this.name = name;
            isDead = false;
        }
        public bool Damaged(int damage)
        {
            if (damage >= hp)
            {
                hp = 0;
                isDead = true;
                return true;
            }
            hp -= damage;
            return false;
        }
        public string GetName() { return name; }
        public int GetHp() { return hp; }
        public int GetAtk() { return atk; }
        public bool IsDead() { return isDead; }
    }
    struct Item
    {
        int itemprice;
        string itemName;
        public Item(string itemName, int itemprice)
        {
            this.itemName = itemName; 
            this.itemprice = itemprice;
        }
        public int GetPrice() { return itemprice; }
        public string GetName() {  return itemName; }
    }
    class Character
    {   //각 캐릭터마다 스탯, 공격, 피격 기본구현
        protected Status stat;
        protected Character(Status stat)
        {
            this.stat = stat;
        }
        public bool Attack(Character target)
        {
            if (target.stat.IsDead())
            {
                Debug.Log("이미 죽은 대상입니다. ");
                return false;
            }
            else
            {
                Debug.Log($"{stat.GetName()}의 공격!");
                return target.Hit(stat.GetAtk());
            }
        }
        public bool Hit(int damage)
        {   //protect로 보호한 후 user클래스에서 오버라이딩 할 수 있는 방법이 있을까?
            if (stat.Damaged(damage))
            {
                Debug.Log($"{stat.GetName()} 사망!");
                return true;
            }
            Debug.Log($"{stat.GetName()} {damage}의 피해, 남은 체력: {stat.GetHp()}");
            return false;
        }

    }

    class User : Character
    {
        int level = 1;
        int exp = 0;
        int userGold = 20;
        Item[] inven;
        public User(Status inputStat, Item[] items) : base(inputStat) { inven = items; }
        public void AttackNpc(Character target)
        {
            if (Attack(target))
            {
                Debug.Log($"{stat.GetName()} 보상 획득!");
                /*target 클래스에서 reward 메소드를 보상으로 주고 싶은데 구현방법을 찾지 못함
                 *
                 * int rewards[] = target.reward();
                Debug.Log($"보상 획득: {rewards[0]}Gold, {rewards[1]}Exp");
                */
            }
        }
        public void PrintInfo()
        {
            Debug.Log($"이름: {stat.GetName()}, 상인\n체력: {stat.GetHp()}, 공격력: {stat.GetAtk()}");
            Debug.Log($"레벨: {level},경험치: {exp}, 소지금: {userGold}");
        }
        public void PrintInven()
        {
            for (int i = 0; i < inven.Length; i++)
            {
                Debug.Log($"{inven[i].GetName()}, ({inven[i].GetPrice()}Gold)");
            }
        }
    }
    class NPC : Character
    {
        int rewardGold;
        int rewardExp;
        int aggroRange;
        public NPC(Status inputStat) : base(inputStat) { }
    }
    class Monster : NPC
    {   //모든 몬스터는 선공을 함
        protected static bool aggressive = true;
        protected Monster(Status inputStat) : base(inputStat){ }

    }
    class Ally : NPC
    {   //보든 조력자 NPC는 비선공
        protected static bool aggressive = false;
        protected Ally(Status inputStat) : base(inputStat) { }
    }
    class Orc : Monster
    {
        static int aggroRange = 10;
        static int rewardExp = 150;
        static int rewardGold = 70;
        public Orc(Status inputStat, int aggroRange) : base(inputStat) { }
        public void PrintInfo()
        {
            Debug.Log($"이름: {stat.GetName()}, 상인\n체력: {stat.GetHp()}, 공격력: {stat.GetAtk()}");
            Debug.Log($"선공여부: {aggressive}\n인식거리: {aggroRange}");
        }
        public int[] reward()
        {
            return new int[] { rewardExp, rewardGold };
        }
    }

    class Merchant : Ally
    {
        Item[] sellingItem;
        int aggroRange;
        public Merchant(Status inputStat, Item[] sellingItem,int aggroRange) : base(inputStat)
        {
            this.sellingItem = sellingItem;
            this.aggroRange = aggroRange;
        }
        public void PrintShop()
        {
            Debug.Log("판매 물품: ");
            for (int i = 0; i < sellingItem.Length; i++)
            {
                Debug.Log($"{sellingItem[i].GetName()} - {sellingItem[i].GetPrice()}Gold");
            }
        }
        public void PrintInfo()
        {
            Debug.Log($"이름: {stat.GetName()}, 상인\n체력: {stat.GetHp()}, 공격력: {stat.GetAtk()}");
            Debug.Log($"선공여부: {aggressive}, 인식거리: {aggroRange}");
        }
    }
    
    private void Start()
    {
        //animalA, animalB 이 변수들은 설계도로 만들어낸 실체(instance),객체
        Item[] merchant = new Item[] { new Item("철검", 150), new Item("나무방패",70), new Item("포션",10), new Item("나무방패",60), new Item("경갑",200) };
        Item[] inven = new Item[] { new Item("목검", 20), new Item("냄비뚜껑", 50), new Item("약초", 5) };
        User player = new User(new Status(500, 75, "레온하르트_옥"), inven);
        Orc orc1 = new Orc(new Status(100, 15, "오크 전사"), 15);
        Orc orc2 = new Orc(new Status(300, 40, "오크 투사"), 25);
        Merchant shylock = new Merchant(new Status(75, 5, "샤일록"), merchant, 5);
        player.PrintInfo();
        orc1.PrintInfo();
        orc2.PrintInfo();
        shylock.PrintInfo();
        player.AttackNpc(orc1);
        player.AttackNpc(orc1);
        player.AttackNpc(orc1);
        shylock.PrintShop();
        orc2.Attack(player);
        //merchant[0] = new Item("철검", 150);
    }

}
