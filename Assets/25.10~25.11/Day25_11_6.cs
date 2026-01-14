/*using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;



public class Day25_11_6 : MonoBehaviour
{
    //FSM 유한 상태 머신
    //유한한 상태가 있고, 이것을 제어하는 방법.
    //상인이 판매하는 아이템 테이블

    enum GameState
    {
        Menu = 0,
        Explore = 1,
        Battle = 2,
        Pause = 3
    }
    public enum ArmorType { Helmet, Armor, Glove, Gaiters, Shoes, Count }
    //헬멧, 갑옷(상의), 장갑, 각반, 신발 순
    public enum WeaponType { Sword, Spear, Club, Count }
    //검, 창, 둔기
    public enum Rarity { Common, Uncommom, Rare, Count }
    public enum MonsterRarity { Normal, Elite, Boss}
    public abstract class Item
    {   //카드의 이름, 내용 텍스트, 코스트, 피해량, 방어도
        protected string Name;
        protected int cost;
        protected Rarity rare;
        public Item(string Name, int cost, Rarity rare)
        {
            this.Name = Name;
            this.cost = cost;
            this.rare = rare;
        }
        abstract public void PrintInfo(int i);
        abstract public void PrintInfo(string s);
        public string GetName() { return Name; }
        public int GetCost() { return cost; }
        public Rarity GetRarity() { return rare; }
    }
    public class Potion : Item
    {
        int recovery;
        public Potion(string Name, int cost, int recovery, Rarity rare) : base(Name, cost, rare)
        {
            this.recovery = recovery;
        }
        public void UseItem(int idx)
        {
            if (Player.GetHp() + recovery > Player.GetMaxHp())
            {
                Debug.Log($"체력 {Player.GetMaxHp() - Player.GetHp()} 회복! \n현재 Hp: {Player.GetMaxHp()}");
                Player.Recovery(recovery);
            }
            else
            {
                Player.Recovery(recovery);
                Debug.Log($"체력 {recovery}만큼 회복! \n현재 Hp: {Player.GetHp()}");
            }
        }
        public override void PrintInfo(int i)
        {
            Debug.Log($"{i}. 이름: {this.Name} ({this.rare}), 회복량: {recovery} , 개당 구매 가격: {this.cost}");
        }
        public override void PrintInfo(string s)
        {
            Debug.Log($"{s}. 이름: {this.Name} ({this.rare}), 회복량: {recovery} , 개당 구매 가격: {this.cost}");
        }


        public int GetRecovery() { return recovery; }
    }
    public abstract class EquipItem : Item
    {
        public EquipItem(string Name, int cost, Rarity rare) : base(Name, cost, rare) { }
    }
    public class Weapon : EquipItem
    {
        int Atk;
        WeaponType type;
        public Weapon(string Name, int cost, int Atk, WeaponType type, Rarity rare) : base(Name, cost, rare)
        {
            this.Atk = Atk;
            this.type = type;
        }
        public int Atk() { return this.Atk; }
        public WeaponType GetWeaponType() { return this.type; }
        public override void PrintInfo(int i)
        {
            Debug.Log($"{i}. 무기 이름: {this.Name} ({rare}), 부위: {type}, 공격력: {Atk}, 가격: {cost}");
        }
        public override void PrintInfo(string s)
        {
            Debug.Log($"{s}. 무기 이름: {this.Name} ({rare}), 부위: {type}, 공격력: {Atk}, 가격: {cost}");
        }
    }
    public class Armor : EquipItem
    {
        ArmorType type;
        int Def;
        public Armor(string Name, int cost, int Atk, ArmorType type, Rarity rare) : base(Name, cost, rare)
        {
            this.Def = Atk;
            this.type = type;
        }
        public int Def() { return this.Def; }
        public ArmorType GetArmorType() { return this.type; }
        public override void PrintInfo(int i)
        {
            Debug.Log($"{i}. 방어구 이름: {this.Name} ({rare}), 부위: {type}, 방어력: {Def}, 가격: {cost}");
        }
        public override void PrintInfo(string s)
        {
            Debug.Log($"{s}. 방어구 이름: {this.Name} ({rare}), 부위: {type}, 방어력: {Def}, 가격: {cost}");
        }
    }
    public class ItemDB
    {
        public static List<Item> AllItems = new List<Item>()
        {   //포션류
            new Potion("빨간 포션", 15,10,Rarity.Common),
            new Potion("약초", 10, 5,Rarity.Common),
            new Potion("주황 포션", 30, 20,Rarity.Uncommom),
            new Potion("하얀 포션", 50,30,Rarity.Uncommom),
            new Potion("엘릭서",70,70,Rarity.Rare),
            //무기
            //검
            new Weapon("목검",300,5,WeaponType.Sword,Rarity.Common),
            new Weapon("하이랜더",1200,12,WeaponType.Sword,Rarity.Uncommom),
            new Weapon("참화도",1800,20,WeaponType.Sword,Rarity.Rare),
            //창
            new Weapon("죽창",300,6,WeaponType.Spear,Rarity.Common),
            new Weapon("나카마키",1050,12,WeaponType.Spear,Rarity.Uncommom),
            new Weapon("피나카",1900,22,WeaponType.Sword,Rarity.Rare),
            //둔기
            new Weapon("나무 망치",300,6,WeaponType.Club,Rarity.Common),
            new Weapon("타이탄",900,13,WeaponType.Club,Rarity.Uncommom),
            new Weapon("골든 스미스해머",2000,23,WeaponType.Club,Rarity.Rare),
            //방어구
            //헬멧
            new Armor("메탈 코이프", 120, 12, ArmorType.Helmet,Rarity.Common),
            new Armor("레드 듀크", 400, 15, ArmorType.Helmet,Rarity.Uncommom),
            new Armor("블랙 니르첸 투구", 700, 19, ArmorType.Helmet,Rarity.Rare),
            //갑주
            new Armor("스틸 코퍼럴", 200, 20, ArmorType.Armor,Rarity.Common),
            new Armor("흑진일갑주", 600, 27, ArmorType.Armor,Rarity.Uncommom),
            new Armor("블랙 네오스", 1000, 35, ArmorType.Armor,Rarity.Rare),
            //장갑
            new Armor("오리할콘 미셀", 130, 12, ArmorType.Glove,Rarity.Common),
            new Armor("미스릴 브리스트", 350, 14, ArmorType.Glove,Rarity.Uncommom),
            new Armor("블랙 코르뱅", 800, 19, ArmorType.Glove,Rarity.Rare),
            //각반
            new Armor("스틸 코퍼럴 바지", 170, 16, ArmorType.Gaiters,Rarity.Common),
            new Armor("흑진일갑주 바지", 400, 17, ArmorType.Gaiters,Rarity.Uncommom),
            new Armor("블랙 네오스 바지", 870, 25, ArmorType.Gaiters,Rarity.Rare),
            //신발
            new Armor("스틸 그리브", 150, 13, ArmorType.Shoes,Rarity.Common),
            new Armor("자 진월장화", 450, 17, ArmorType.Shoes,Rarity.Uncommom),
            new Armor("블루 드래곤 부츠", 820, 22, ArmorType.Shoes,Rarity.Rare)
        };

        public static List<int>[] PotionList = new List<int>[(int)Rarity.Count];
        public static List<int>[] WeaponList = new List<int>[(int)Rarity.Count];
        public static List<int>[] ArmorList = new List<int>[(int)Rarity.Count];

        public static void Initial()
        {
            for (int i = 0; i < (int)Rarity.Count; i++)
            {
                PotionList[i] = new List<int>();
                WeaponList[i] = new List<int>();
                ArmorList[i] = new List<int>();
            }

            for (int i = 0; i < AllItems.Count; i++)
            {   //AllItem 리스트의 첨자를 저장
                //새로운 카테고리 (ex. 유물, 악세서리 등...) 추가시 조건문만 하나 추가하면됨
                Item item = AllItems[i];
                if (item is Potion)
                {
                    PotionList[(int)item.GetRarity()].Add(i);
                }
                else if (item is Weapon)
                {
                    WeaponList[(int)item.GetRarity()].Add(i);
                }
                else if (item is Armor)
                {
                    ArmorList[(int)item.GetRarity()].Add(i);
                }
                else
                {
                    Debug.LogError("장비 인덱싱 오류");
                }
            }
            //PrintResult(); 테스트용 결과출력
        }
        public void PrintResult()
        {   //결과 테스트용
            for (int i = 0; i < 3; i++)
            {
                Debug.Log($"포션 레어리티: {(Rarity)i}");
                for (int j = 0; j < PotionList[i].Count; j++)
                {
                    AllItems[PotionList[i][j]].PrintInfo(j + 1);
                }
            }
            for (int i = 0; i < 3; i++)
            {
                Debug.Log($"무기 레어리티: {(Rarity)i}");
                for (int j = 0; j < WeaponList[i].Count; j++)
                {
                    AllItems[WeaponList[i][j]].PrintInfo(j + 1);
                }
            }
            for (int i = 0; i < 3; i++)
            {
                Debug.Log($"방어구 레어리티: {(Rarity)i}");
                for (int j = 0; j < ArmorList[i].Count; j++)
                {
                    AllItems[ArmorList[i][j]].PrintInfo(j + 1);
                }
            }
        }
    }
    public class Store
    {
        Potion[] sellPotion;
        Item[] sellEquip;
        bool[] soldOutPotion;
        bool[] soldOutEquip;
        //common = 60%, uncommon = 30%, rare = 10%
        int[] rarityWeight = { 60, 30, 10 };
        //무기 등장확률 30%, 방어구 등장확률 70%
        int[] equipTypeWeight = { 30, 70 };

        int random;
        public Store(int potion, int equip)
        {
            sellPotion = new Potion[potion];
            soldOutPotion = new bool[potion];
            sellEquip = new Item[equip];
            soldOutEquip = new bool[equip];
            TakePotion();
            TakeEquip();
            PrintHello();
            PrintInfo();
        }
        public void PrintHello()
        {
            Debug.Log("상점에 온 것을 환영하네! 자네에게 딱 맞는 물건이 있다네!");

        }
        public void PrintInfo()
        {
            Debug.Log("포션류:");
            for (int i = 0; i < sellPotion.Length; i++)
            {
                sellPotion[i].PrintInfo(i + 1);
            }
            Debug.Log("장비류:");
            for (int i = 0; i < sellEquip.Length; i++)
            {
                sellEquip[i].PrintInfo(i + 1);
            }
            Debug.Log("구매하고 싶은 아이템이 있나? \n1. 포션 2. 장비");
            //여기서 키보드 입력받아서 BuyPotion / BuyItem으로 이동
        }
        public int CalcWeight(int[] weight)
        {   //weight의 첨자가 낮을수록 낮은 희귀도
            //weight 실수값, 0, 음수 입력금지 
            int total = 0;
            for (int i = 0; i < weight.Length; i++)
            {
                total += weight[i];
                if (weight[i] == 0 && weight[i] < 0)
                {
                    Debug.LogWarning($"{weight[i]}는 잘못된 입력입니다 (0이나 음수 입력) ");
                }
            }
            int random = UnityEngine.Random.Range(0, total);
            for (int i = 0; i < weight.Length; i++)
            {
                if (random >= weight[i])
                {
                    random -= weight[i];
                }
                else return i;
            }
            Debug.Log("입력값 오류");
            return 0;
        }
        public void TakePotion()
        {
            int rareity;
            //무한루프 방지용
            int cnt = 0;
            for (int i = 0; i < sellPotion.Length && cnt <= 80; cnt++)
            {
                rareity = CalcWeight(rarityWeight);
                //rareity 희귀도번째 PotionList를 불러옴
                List<int> itemPool = ItemDB.PotionList[rareity];
                if (itemPool.Count <= 0)
                {
                    Debug.Log($"{(Rarity)rareity}등급의 포션이 없습니다");
                }
                else
                {
                    int idx = itemPool[UnityEngine.Random.Range(0, itemPool.Count)];
                    sellPotion[i++] = (Potion)ItemDB.AllItems[idx];
                }
            }

        }
        public void TakeEquip()
        {
            int rareity;
            int type;
            int cnt = 0;
            for (int i = 0; i < sellEquip.Length && cnt <= 80; cnt++)
            {
                type = CalcWeight(equipTypeWeight);
                rareity = CalcWeight(rarityWeight);
                if (type == 0)
                { //무기
                    List<int> itemPool = ItemDB.WeaponList[rareity];
                    if (itemPool.Count <= 0)
                    {
                        Debug.Log($"{(Rarity)rareity}등급의 무기가 없습니다");
                    }
                    else
                    {
                        int idx = itemPool[UnityEngine.Random.Range(0, itemPool.Count)];
                        sellEquip[i++] = (Weapon)ItemDB.AllItems[idx];
                    }
                }
                else
                { //장비
                    List<int> itemPool = ItemDB.ArmorList[rareity];
                    if (itemPool.Count <= 0)
                    {
                        Debug.Log($"{(Rarity)rareity}등급의 방어구가 없습니다");
                    }
                    else
                    {
                        int idx = itemPool[UnityEngine.Random.Range(0, itemPool.Count)];
                        sellEquip[i++] = (Armor)ItemDB.AllItems[idx];
                    }
                }
            }
        }
        public void BuyPotion(int input)
        {
            //나중에는 input지우로 여기서 키 입력받아서 구매하는식으로
            Debug.Log("포션을 구매하려고? 잘 생각했네! ");
            for (int i = 0; i < sellPotion.Length; i++)
            {
                if (soldOutPotion[i]) { Debug.Log($"{i + 1}. 품절"); }
                else { sellPotion[i].PrintInfo(i + 1); }
            }
            Debug.Log("포션 선택:                       (되돌아가기: 0)");
            //여기서 키보드 입력받아서 input으로 받기
            if (input == 0)
            {
                PrintInfo();
                //fsm컴포넌트 변경?
                return;
            }
            if(input<=0 && input > sellPotion.Length)
            {//keydown내부 int 캐스팅해서 크기비교 
                Debug.LogWarning($"잘못된 입력입니다. (input{input})");
                return;
            }
            if (Player.GetGold() < sellPotion[input - 1].GetCost())
            {
                Debug.Log($"{sellPotion[input - 1].GetName()}을 사기에는 돈이 부족한것 같은데?");
                return;
            }
            if (soldOutPotion[input - 1])
            {
                Debug.Log("이미 팔린 물품이라네!");
                //BuyPotion(); -> fsm구현 시
                return;
            }
            if (Player.AddItem(sellPotion[input - 1]))
            {//인벤토리에 공간이 있을 시 구매 후 골드 차감
                Player.UseGold(sellPotion[input - 1].GetCost());
                soldOutPotion[input - 1] = true;
                Debug.Log($"{sellPotion[input - 1].GetName()}을 구매했다네! \n남은 소지금: {Player.GetGold()}");
            }
            else
            {
                Debug.Log("인벤토리에 공간이 없네!");
            }

        }
        public void BuyEquip(int input)
        {
            //나중에는 input지우로 여기서 키 입력받아서 구매하는식으로
            Debug.Log("장비를 구매하려고? 잘 생각했네! ");
            for (int i = 0; i < sellEquip.Length; i++)
            {
                if (soldOutEquip[i]) { Debug.Log($"{i + 1}. 품절"); }
                else { sellEquip[i].PrintInfo(i + 1); }
            }
            Debug.Log("장비 선택:                       (되돌아가기: 0)");
            //여기서 키보드 입력받아서 input으로 받기
            if (input == 0)
            {
                PrintInfo();
                //fsm컴포넌트 변경?
                return;
            }
            if (input <= 0 || input > sellEquip.Length)
            {//keydown내부 int 캐스팅해서 크기비교 
                Debug.LogWarning($"잘못된 입력입니다. (input{input})");
                return;
            }
            if (Player.GetGold() < sellEquip[input - 1].GetCost())
            {
                Debug.Log($"{sellEquip[input - 1].GetName()}을 사기에는 돈이 부족한것 같은데?");
                return;
            }
            if (soldOutEquip[input - 1])
            {
                Debug.Log("이미 팔린 물품이라네!");
                //BuyPotion(); -> fsm구현 시
                return;
            }
            if (Player.AddItem(sellEquip[input - 1]))
            {//인벤토리에 공간이 있을 시 구매 후 골드 차감
                Player.UseGold(sellEquip[input - 1].GetCost());
                soldOutEquip[input - 1] = true;
                Debug.Log($"{sellEquip[input - 1].GetName()}을 구매했다네! \n남은 소지금: {Player.GetGold()}");
            }
            else
            {
                Debug.Log("인벤토리에 공간이 없네!");
            }
        }
        public void SellItem(int input)
        {
            //input 지우고 fsm으로 변환
            Debug.Log("판매할 아이템");
            Player.PrintInventory();
            //키 입력
            if (input > Player.GetInventoryCount() || input<0)
            {
                Debug.LogWarning($"인벤토리 외부의 인덱스를 참조하고 있습니다 {input}");
                return; //->SellItem();
            }
            else
            {
                Player.GetPickItem(input).PrintInfo("판매완료! ");
                Debug.Log($"획득 골드: {(int)(Player.GetPickItem(input).GetCost() * 0.7)}");
                Player.UseGold(-1*(int)(Player.GetPickItem(input).GetCost() * 0.7));
                Player.RemoveItem(input);
            }
        }
    }
    public class Inventory
    {
        List<Item> items;
        int maxSize;
        public Inventory(int size)
        {
            maxSize = size;
            items = new List<Item>();
        }
        public bool AddItem(Item item)
        { //인벤토리에 아이템을 넣기만 함
            if (items.Count < maxSize)
            {
                items.Add(item);
                return true;
            }
            else
            {
                Debug.Log("인벤토리 공간 초과");
                return false;
            }
        }
        public int GetCount() { return  items.Count; }
        public void Replace(int idx, Item item) { items[idx] = item; }
        //특정 인덱스에 있는 장비를 item 객체로 전환(장비교체 구현용)
        public void Remove(int idx) { items.RemoveAt(idx); }
        //아이템 사용시 제거
        public Item Pick(int idx) { return items[idx]; }
        //특정 인덱스의 Item인덱스 반환
        public void PrintInventory()
        {
            Debug.Log("인벤토리 확인");
            for (int i = 0; i < items.Count; i++) { items[i].PrintInfo(i); }
        }
        public void PrintInventorySlot(int idx)
        {
            if (0 <= idx && idx < items.Count) { items[idx].PrintInfo(idx); }
            else { Debug.LogWarning($"{idx}번째 칸의 장비는 없습니다. "); }
        }
    }
    public struct Stat
    {
        string Name;
        int Hp;
        int MaxHp;
        int Atk;
        int Def;
        bool IsDead;
        public Stat(string Name, int Hp, int Atk, int Def)
        {
            this.Name = Name;
            if (Hp <= 0) this.Hp = 10;
            else this.Hp = Hp;
            this.MaxHp = Hp;
            if (Atk <= 0) this.Atk = 1;
            else this.Atk = Atk;
            if (Def <= 0) this.Def = 1;
            else this.Def = Def;
            IsDead = false;
        }
        public int GetHp() { return Hp; }
        public int Atk() { return Atk; }
        public int Def() { return Def; }
        public string GetName() { return Name; }
        public bool IsDead() { return this.IsDead; }

        public void Recovery(int recovery)
        {
            if (Hp + recovery > MaxHp) Hp = MaxHp;
            else Hp += recovery;
        }
        public bool Damaged(int damage)
        {
            if (damage >= Hp)
            {
                Debug.Log($"{this.Name}은 사망했습니다");
                this.IsDead = true;
                this.Hp = 0;
                return true;
            }
            if (damage < 0) damage = 0;
            Hp -= damage;
            return false;
        }
        public void ChangeAtk(int change)
        {
            //공격력이 감소하더라도 최소 1 보장
            if (Atk + change <= 0) { this.Atk = 1; }
            else { this.Atk += change; }
        }
        public void ChangeDef(int change)
        {
            if (Def + change < 0) { this.Def = 0; }
            else { this.Def += change; }
        }
        public void ChangeMaxHp(int change)
        {
            MaxHp += change;
            Hp = MaxHp;
        }
    }
    public static class Player
    {
        //구조체로 변환필요
        static Stat stat;
        static int MaxHp;
        static int TotalAtk;
        static int TotalDef; 
        static int Gold;
        static int[] armorDef = new int[5];
        static Inventory inven;
        static Weapon equipWeapon = null;
        static Armor[] equipArmor = new Armor[] { null, null, null, null, null };
        //enum데이터 ArmorType으로 관리
        //0 -> Helmet, 1-> Armor, 2-> Glove, 3-> Gaiters, 4-> Shoes

        public static void Initial(Stat _stat, int _maxinventorySize, int _startgold)
        {
            stat = _stat;
            MaxHp = stat.GetHp();
            TotalAtk = stat.Atk();
            TotalDef = stat.Def();
            Gold = _startgold;
            inven = new Inventory(_maxinventorySize);
        }
        public static int GetMaxHp() { return MaxHp; }
        public static int GetGold() { return Gold; }
        public static int GetHp() { return stat.GetHp(); }
        public static int GetInventoryCount() { return inven.GetCount(); }
        public static Item GetPickItem(int input) { return inven.Pick(input); }
        public static void Recovery(int recovery) { stat.Recovery(recovery); }
        public static bool Hit(int damage)
        {
            if (stat.Damaged(damage))
            {
                //여기다가 플레이어가 죽었을 때 게임이 끝나는 함수를 넣어볼까
                return true;
            }
            return false;
        }
        public static void UseGold(int consume) { Gold -= consume; }
        public static void PrintInventory() { inven.PrintInventory(); }
        public static void PrintEquipItem()
        {   //장착한 장비 확인
            if (equipWeapon == null) { Debug.Log("장착 무기: 없음"); }
            else { Debug.Log($"장착 무기: {equipWeapon.GetName()}, 장비 공격력: {equipWeapon.Atk()}\n총 공격력: {TotalAtk}"); }
            for (int i = 0; i < 5; i++)
            {
                if (equipArmor[i] == null) { Debug.Log($"{(ArmorType)i}부위 착용 방어구: 없음"); }
                else { Debug.Log($"{(ArmorType)i}부위 착용 방어구: {equipArmor[i].GetName()}, 장비 방어구: {equipArmor[i].Def()}"); }
            }
            Debug.Log($"총 방어력: {TotalDef}");
        }
        public static void PrintInfo()
        {   //캐릭터 정보 확인
            Debug.Log($"소지금: {Gold}, 기본공격력: {stat.Atk()}, 총 공격력: {TotalAtk}, 기본방어력: {stat.Def()}, 총 방어력: {TotalDef}");
        }
        public static void UseItem(int idx)
        {   //아이템 사용(포션/방어구/무기 분기)
            Item usingItem = inven.Pick(idx);
            if (usingItem is Weapon)
            {   //선택하려고 하는 장비가 무기일 경우
                Weapon usingWeapon = (Weapon)usingItem;
                if (equipWeapon == null)
                {
                    //장착한 무기가 없으면 장착하고 인벤에서 제거
                    equipWeapon = usingWeapon;
                    inven.Remove(idx);
                }
                else
                {
                    inven.Replace(idx, equipWeapon);
                    equipWeapon = usingWeapon;
                }
                TotalAtk = equipWeapon.Atk() + stat.Atk();
                Debug.Log($"{usingWeapon.GetName()} ({usingWeapon.GetWeaponType()})장착\n 추가 공격력: {usingWeapon.Atk()}, 총 공격력: {TotalAtk}");
            }
            if (usingItem is Armor)
            {   //선택하려고 하는 장비가 방어구인 경우
                Armor usingArmor = (Armor)usingItem;
                ArmorType type = usingArmor.GetArmorType();
                if (equipArmor[(int)type] == null)
                //장착하려는 타입의 방어구를 장착하고있지 않을 시 장착하고 인벤에서 제거
                //장착하려는 usingItem 객체의 ArmorType을 정수로 캐스팅
                //현재 플레이어는 장착 장비를 enum형식을 이용해 배열로 저장하고 있음
                //0 -> Helmet, 1-> Armor, 2-> Glove, 3-> Gaiters, 4-> Shoes
                {
                    equipArmor[(int)type] = (Armor)usingItem;
                    inven.Remove(idx);
                }
                else
                { //해당 타입의 방어구 장착 시 기존아이템-> 인벤, 장착하려는 아이템 장착
                    inven.Replace(idx, equipArmor[(int)type]);
                    equipArmor[(int)type] = (Armor)usingItem;
                }
                TotalDef = equipArmor[(int)type].Def() + stat.Def();
                Debug.Log($"{equipArmor[(int)type].GetName()} ({type})장착\n 추가 방어력: {equipArmor[(int)type].Def()}, 총 방어력: {TotalDef}");

            }

        }
        public static bool AddItem(Item item) { return inven.AddItem(item); }
        public static void RemoveItem(int input) { inven.Remove(input); }
    }
    abstract class Monster
    {
        //노말 몬스터는 2개, 엘리트 몬스터는 3가지의 스킬 존재
        protected Stat stat;
        protected MonsterRarity rarity;
        public Monster(Stat stat, MonsterRarity rarity)
        {
            this.stat = stat;
            this.rarity = rarity;
        }
        public void RandomStat()
        {
            int randAtk = UnityEngine.Random.Range(-2,2);
            stat.ChangeAtk(randAtk);
            int randDef = UnityEngine.Random.Range(-2,2);
            stat.ChangeDef(randDef);
            int randHp = UnityEngine.Random.Range(-5,10);
            stat.ChangeMaxHp(randHp);
        }
        abstract public void UseSkill();
        abstract public void Skill1();
        abstract public void Skill2();
    }
    abstract class EliteMonster : Monster
    {
        public EliteMonster(Stat stat, MonsterRarity rarity) : base(stat, rarity)
        {

        }
        abstract public void Skill3();

    }
    abstract class NormalMonster : Monster
    {
        public NormalMonster(Stat stat, MonsterRarity rarity) : base(stat, rarity) 
        {

        }
    }
    
    class Thief : NormalMonster
    {
        public Thief() :base(new Stat("도둑",30,5,1), MonsterRarity.Normal)
        {
            this.RandomStat();
        }
        
        public override void UseSkill()
        {
            
        }
        public override void Skill1()
        {

        }
        public override void Skill2()
        {

        }
        public void Print()
        {
            Debug.Log($"{stat.GetName()},{stat.Atk()},{stat.Def()},{stat.GetHp()},{rarity}");
        }
    }

    private void Start()
    {
        ItemDB.Initial();
        Stat playerStat = new Stat("player", 100, 12, 5);
        Player.Initial(playerStat, 10, 10000);

        Player.PrintInventory();
        Player.PrintInfo();
        Store store = new Store(5, 9);
        store.BuyPotion(2);
        store.BuyPotion(5);
        store.BuyEquip(5);
        store.BuyEquip(1);
        store.BuyEquip(9);
        store.BuyEquip(0);
        store.BuyEquip(9);
        store.BuyEquip(10);
        Player.PrintInfo();

        store.SellItem(2);
        Player.PrintInfo();
        Player.PrintInventory();
        Thief thief = new Thief();

        thief.Print();
    }
    private void Update()
    {

    }

}



최종 과제 구현목표
1. 게임 타입 - 슬더슬구조, 전투는 림버스+쯔꾸르
2. 게임 내부 
2-1. 시작: 캐릭터 시작 선택지 구현(고래)
2-2. 선택지:  일반전투, 엘리트 전투, 이벤트, 상점, 휴식 5가지
2-3. 전투구조: 일반몬스터와 엘리트 몬스터가 존재하고 ,각 전투 조우는 프리셋된 몬스터 묶음에서 랜덤으로 조우
2-4. 전투구조: 공격 타입은 3가지로, 각 적마다 내성이 존재, 전투진입 전 어떤 적들이 나올지 알려주고 장비를 바꿀 기회를 줌 
2-5. 이벤트구조: (강화,제거,변화) 3가지 + 특수이벤트(특정 카드/골드 소모해 보상획득 / 선택지 골라서 운에따라 결과발생)
2-6. 스테이지: 이번주 과제에서는 일단 1스테이지만 만들고 8번의 경로를 선택한 후 상점 -> 휴식고정 -> 보스
오늘 과제 구현목표
1. 상점 들어가기 , 아이템 구매 V
1-1. 아이템은 3가지 (무기, 갑옷, 물약) V
1-2. 무기는 검(참격), 창(관통), 둔기(타격) △
1-3. 각 무기마다 3개의 스킬(스킬1 -> 약한 공격 / 스킬2 -> 방어 / 스킬3 -> 강한 공격) X
금요일 과제 구현목표
1. 목요일 만든 코드 FMS로 변환
2. Player 클래스 최적화(각종 변수 구조체로 통일) O
3. 전투 시스템 구현 (각 전투는 1-1)
4. 전투 시스템 FMS로 변환
주말 내 과제 구현목표
1. 랜덤 분기 그리드 생성(이벤트,일반전투,정예전투,상점,휴식) -> 제한을 통해 예상외의 구조(연속 2번 상점, 1스테이지부터 정예전투...) 제외
2. stage 1 구현(8개 맵 -> 상점 -> 휴식 -> 보스전투)
2-1. 각 전투마다 1-다수 전투 구현
3. 장비 강화 구현
4. 레벨업에 따른 스펙업 디자인 계획
5. 난이도 디자인 공부
6. 다양한 이벤트 구현(객체로? 함수로?)
 */
