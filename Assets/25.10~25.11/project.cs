using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static UnityEngine.GraphicsBuffer;



public class project : MonoBehaviour
{
    /*
    11.10 - 1 패치노트
    1. 상점에서 0번 포션을 사용할 수 없었던 버그 수정
    2. 포션 사용 시 2번 회복되는 버그 수정
    3. 전투 승리 시 보상골드 미지급 버그 수정
    4. 상점 내 아이템 판매 불가 버그 수정
    5. 피나카 무기 분류 오류 수정(검->창)

    11.10 - 2 패치노트
    1. Gold 변동 UsePool/Add 분리
    2. 난이도 추가 
    3. 장비/포션 판매 간 품절 아이템 확인로직 변경(flag배열 -> sellPotion/Equip null 확인)
    4. update 내 if문 if-elseif 구조로 변경
    5. Inventory 클래스 Replace index 가드

    11.11 - 1 패치노트
    1. 팩토리 이용한 무작위 몬스터 등장 구현
    2. 피격 판정과 사망판정 분리(Stat 클래스 Hit 메소드에서 사망시 텍스트만 출력하고 생/사여부 반환하지 않음)
    3. 인터페이스를 이용해 몬스터들의 스킬을 추가(보스 몬스터 3개, 엘리트 몬스터 2개, 노말 몬스터 2개),
       각 스킬을 랜덤한 확률로 사용 구현
    4. 게임 FSM 상태 간 구분선 추가하여 가시성 확보

    11.12 - 1 패치노트
    1. 변수 네이밍 규칙에 따른 이름 변경
    2. 무기 종류에 따른 스킬 사용 구현
    3. 코드 정리 및 주석처리, 변수명 정리

    11.13 - 1 패치노트
    1. 치명타, 회피 QTE 구현
    2. 난이도 세분화(기초스텟, 시작골드 + QTE 난이도 차등)

    패치예정
    1. 맵 그리드 제작해서 무작위 맵 선택 구현
    2. 이벤트 및 휴식맵 추가
    3. (심화) 아티펙트 추가(기반 스텟에 영향을 주거나 시작 AP증가, 적군 고급스킬 발동확률 감소 등...)
     */


    /*----------------------------상점 기능 구현부----------------------------*/
    /// <summary>
    /// 가중합 방식으로 확률을 계산, 확률이 0이어도 가능해 확률조작 가능
    /// </summary>
    /// <param name="weight">각 인덱스의 등장 비중(합이 100이 아니어도 되지만 음수는 입력불가)</param>
    /// <returns></returns>
    public static int CalcWeight(int[] weight)
    {
        //계산은 누적합 방식으로 계산
        int total = 0;
        for (int i = 0; i < weight.Length; i++)
        {
            if (weight[i] < 0)
            {
                Debug.LogWarning($"가중치 입력이 잘못되었습니다 weight[{i}] = {weight[i]}");
                return 0;
            }
            total += weight[i];
            //오류 발생 시 최하등급 부여
        }
        if (total == 0)
        {
            Debug.LogWarning("가중치 배열의 합이 0입니다!");
            return 0;
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

    /// <summary>
    /// 게임 내 등장하는 상점.
    /// 상점 진입시마다 새로운 아이템이 등장
    /// </summary>
    public class Store
    {
        //상점에 진열될 포션 배열(TakePotion()함수에서 생성됨) - 없으면 null
        private readonly Potion[] sellPotion;
        //상점에 진열될 장비 배열(TakeEquip()함수에서 생성됨) - 없으면 null
        private readonly Item[] sellEquip;
        /// <summary>
        /// 등장 아이템의 등급 확률.
        /// 배열의 값은 해당 enum Rarity 인덱스의 등장확률
        /// Common = 60%, Uncommon = 30%, Rare = 10%
        /// 게임 난이도 조절 필요 시 변경
        /// </summary>
        private int[] rarityWeight = { 60, 30, 10 };
        /// <summary>
        /// 착용장비 중 무기, 방어구 등장 확률. 현재는 무기 30%, 방어구 70%
        /// </summary>
        private int[] equipTypeWeight = { 30, 70 };

        /// <summary>
        /// 배열에 진열될 포션과 장비의 수를 받아서 
        /// 해당 갯수만큼 아이템을 뽑아 상점만들기
        /// </summary>
        /// <param Name="potion">상점에서 판매하는 포션의 개수</param>
        /// <param Name="equip">상점에서 판매하는 장비의 개수</param>
        public Store(int potion, int equip)
        {
            sellPotion = new Potion[potion];
            sellEquip = new Item[equip];
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
            Debug.Log("무엇을 하고싶나? \n1. 포션 구매 2. 장비 구매 3. 아이템 판매 Back Space. 상점 나가기 ");
            //여기서 키보드 입력받아서 BuyPotion / BuyItem으로 이동
        }
        /// <summary>
        /// 가중치 배열을 넣어 해당 확률에 따른 결과 리턴
        /// 입력 시 가중치배열에는 0, 음수 입력 금지
        /// </summary>
        /// <param Name="weight">가중치 배열(종류무관)</param>
        /// <returns>가중치 배열의 특정 첨자(희귀도)</returns>

        /// <summary>
        /// 가중치 배열에 따른 포션 진열
        /// </summary>
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
        /// <summary>
        /// 가중치 배열에 따른 장비 진열
        /// </summary>
        private void TakeEquip()
        {
            int rareity;
            //장비 종류 (무기 / 방어구)
            int type;
            int cnt = 0;
            for (int i = 0; i < sellEquip.Length && cnt <= 80; cnt++)
            {
                type = CalcWeight(equipTypeWeight);
                rareity = CalcWeight(rarityWeight);
                if (type == 0)
                { //무기 생성
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
        public void BuyPotionPrint()
        {

            for (int i = 0; i < sellPotion.Length; i++)
            {
                if (sellPotion[i] == null) { Debug.Log($"{i}. 품절"); }
                else { sellPotion[i].PrintInfo(i); }
            }
            Debug.Log("포션 선택                      (되돌아가기: Back Space)");
        }
        /// <summary>
        /// 키보드 숫자를 입력받아서 해당 포션 구매
        /// </summary>
        /// <param Name="input">OnStore()상태에서 입력받은 키 값</param>
        /// <param Name="player">포션을 구매하는 Player 객체</param>
        public void BuyPotion(int input, Player player)
        {
            if (input < 0 || input >= sellPotion.Length)
            {//keydown내부 int 캐스팅해서 크기비교 
                Debug.LogWarning($"(input{input})번째 선반에는 아직 포션을 진열하지 못했네! ");
                return;
            }
            if (sellPotion[input] == null)
            {
                Debug.Log("이미 팔린 물품이라네!");
                //BuyPotion(); -> fsm구현 시
                return;
            }
            if (player.GetGold() < sellPotion[input].GetCost())
            {
                Debug.Log($"{sellPotion[input].GetName()}을 사기에는 돈이 부족한것 같은데?");
                return;
            }

            //인벤토리에 공간이 있을 시 구매 후 골드 차감
            if (player.AddItem(sellPotion[input]))
            {
                player.UseGold(sellPotion[input].GetCost());
                Debug.Log($"{sellPotion[input].GetName()}을 구매했다네! 더 구매하고 싶은게 있나?\n남은 소지금: {player.GetGold()}");
                sellPotion[input] = null;
            }
            else
            {
                Debug.Log("인벤토리에 공간이 없네!");
            }

        }
        public void BuyEquipPrint()
        {
            for (int i = 0; i < sellEquip.Length; i++)
            {
                if (sellEquip[i] == null) { Debug.Log($"{i}. 품절"); }
                else { sellEquip[i].PrintInfo(i); }
            }
            Debug.Log("장비 선택:                       (되돌아가기: Back Space)");
        }
        public void BuyEquip(int input, Player player)
        {
            //나중에는 input지우로 여기서 키 입력받아서 구매하는식으로
            //여기서 키보드 입력받아서 input으로 받기

            if (input < 0 || input >= sellEquip.Length)
            {//keydown내부 int 캐스팅해서 크기비교 
                Debug.LogWarning($"input{input})번째 선반에는 아직 포션을 진열하지 못했네!");
                return;
            }
            if (sellEquip[input] == null)
            {
                Debug.Log("이미 팔린 물품이라네!");
                //BuyPotion(); -> fsm구현 시
                return;
            }
            if (player.GetGold() < sellEquip[input].GetCost())
            {
                Debug.Log($"{sellEquip[input].GetName()}을 사기에는 돈이 부족한것 같은데?");
                return;
            }
            if (player.AddItem(sellEquip[input]))
            {//인벤토리에 공간이 있을 시 구매 후 골드 차감
                player.UseGold(sellEquip[input].GetCost());
                Debug.Log($"{sellEquip[input].GetName()}을 구매했다네! \n남은 소지금: {player.GetGold()}");
                sellEquip[input] = null;
            }
            else
            {
                Debug.Log("인벤토리에 공간이 없네!");
            }
        }
        public void SellItemPrint(Player player)
        {
            Debug.Log("어떤 장비를 판매하고 싶나? 판매하는 장비는 70%의 가격으로 구매해주겠네!\n(판매취소: BackSpace)");
            player.PrintInventory();
        }
        public void SellItem(int input, Player player)
        {
            //input 지우고 fsm으로 변환
            //키 입력
            if (input >= player.GetInventoryCount() || input < 0)
            {
                Debug.LogWarning($"인벤토리 {input}번째 공간에는 장비가 없는것 같은데?");
                return; //->SellItem();
            }
            else
            {
                player.GetPickItem(input).PrintInfo("판매완료! ");
                Debug.Log($"획득 골드: {(int)(player.GetPickItem(input).GetCost() * 0.7)}");
                player.AddGold((int)(player.GetPickItem(input).GetCost() * 0.7));
                player.RemoveItem(input);
            }
        }
    }
    /*----------------------------상점 기능 구현부----------------------------*/

    /*----------------------------아이템 구현부----------------------------*/
    public enum ArmorType { Helmet, Armor, Glove, Gaiters, Shoes, Count }
    //헬멧, 갑옷(상의), 장갑, 각반, 신발 순
    public enum WeaponType { Sword, Spear, Club, Count }
    //검, 창, 둔기
    public enum Rarity { Common, Uncommom, Rare, Count }
    public struct ItemInfo
    {
        /// <summary>
        /// Item 클래스의 이름, 비용, 희귀도를 저장
        /// </summary>
        public string Name { get; private set; }
        public int Cost { get; private set; }
        public Rarity Rare { get; private set; }
        public ItemInfo(string name, int cost, Rarity rare)
        {
            this.Name = name;
            this.Cost = cost;
            this.Rare = rare;
        }
        public void SetName(string name) { this.Name = name; }
        public void SetCost(int cost)
        {
            if (cost <= 0) this.Cost = 1;
            else this.Cost = cost;
        }
        public void SetRarity(Rarity rare)
        {
            if ((int)rare >= (int)Rarity.Count || (int)rare < 0) this.Rare = Rarity.Common;
            else this.Rare = rare;
        }
    }
    public abstract class Item
    {
        protected ItemInfo info;
        /// <summary>
        /// 종류와 관계없는 Item의 기초정보
        /// </summary>
        /// <param Name="Name">Item의 이름</param>
        /// <param Name="cost">Item 구매 비용</param>
        /// <param Name="rare">Item 희귀도</param>
        public Item(string name, int cost, Rarity rare)
        {
            info = new ItemInfo(name, cost, rare);
        }
        abstract public void PrintInfo(int i);
        abstract public void PrintInfo(string s);
        public string GetName() { return info.Name; }
        public int GetCost() { return info.Cost; }
        public Rarity GetRarity() { return info.Rare; }
    }
    public class Potion : Item
    {
        private readonly int recovery;
        /// <summary>
        /// Item클래스의 변수 + 회복량 추가
        /// </summary>
        /// <param Name="recovery">포션의 회복량</param>
        public Potion(string name, int cost, int recovery, Rarity rare) : base(name, cost, rare)
        {
            this.recovery = recovery;
        }
        public int Use()
        {   //회복량만 반환하고 회복, 사용후 제거 등 로직은 호출한 클래스에서 알아서
            return recovery;
        }
        public override void PrintInfo(int i)
        {
            Debug.Log($"{i}. 이름: {info.Name} ({info.Rare}), 회복량: {recovery} , 개당 구매 가격: {info.Cost}");
        }
        public override void PrintInfo(string s)
        {
            Debug.Log($"{s}. 이름: {info.Name} ({info.Rare}), 회복량: {recovery} , 개당 구매 가격: {info.Cost}");
        }
        public int GetRecovery() { return recovery; }
    }

    /// <summary>
    /// 아이템 중 장착 가능한 장비류
    /// </summary>
    public abstract class EquipItem : Item
    {
        public EquipItem(string name, int cost, Rarity rare) : base(name, cost, rare) { }
    }
    public class Weapon : EquipItem
    {
        public int Atk { get; private set; }
        public WeaponType Type { get; private set; }
        /// <summary>
        /// 장착 가능한 장비 중 무기
        /// </summary>
        /// <param Name="Atk"> 무기의 공격력 </param>
        /// <param Name="type"> 무기의 종류 (칼, 창, 둔기) </param>
        public Weapon(string name, int cost, int atk, WeaponType type, Rarity rare) : base(name, cost, rare)
        {
            this.Atk = atk;
            this.Type = type;
        }
        public override void PrintInfo(int i)
        {
            Debug.Log($"{i}. 무기 이름: {info.Name} ({info.Rare}), 부위: {Type}, 공격력: {Atk}, 가격: {info.Cost}");
        }
        public override void PrintInfo(string s)
        {
            Debug.Log($"{s}. 무기 이름: {info.Name} ({info.Rare}), 부위: {Type}, 공격력: {Atk}, 가격: {info.Cost}");
        }
    }

    public class Armor : EquipItem
    {
        public ArmorType Type { get; private set; }
        public int Def { get; private set; }
        /// <summary>
        /// 창착가능한 장비 중 방어구
        /// </summary>
        /// <param Name="Def">방어구의 방어력</param>
        /// <param Name="type">방어구의 종류(헬멧, 상의, 각반, 신발, 장갑)</param>
        public Armor(string name, int cost, int def, ArmorType type, Rarity rare) : base(name, cost, rare)
        {
            this.Def = def;
            this.Type = type;
        }
        public override void PrintInfo(int i)
        {
            Debug.Log($"{i}. 방어구 이름: {info.Name} ({info.Rare}), 부위: {Type}, 방어력: {Def}, 가격: {info.Cost}");
        }
        public override void PrintInfo(string s)
        {
            Debug.Log($"{s}. 방어구 이름: {info.Name} ({info.Rare}), 부위: {Type}, 방어력: {Def}, 가격: {info.Cost}");
        }
    }

    /// <summary>
    /// 게임 내 사용할 아이템 정보 저장하는 DB
    /// </summary>
    public static class ItemDB
    {
        /// <summary>
        /// 아이템 DB, 추가하고 싶은 장비 있으면 양식맞춰서 해당 리스트에 추가 시 게임 내 등장
        /// </summary>
        public static List<Item> AllItems = new List<Item>()
        {   //포션류
            new Potion("약초", 50, 5,Rarity.Common),
            new Potion("빨간 포션", 120,10,Rarity.Common),
            new Potion("주황 포션", 200, 20,Rarity.Uncommom),
            new Potion("하얀 포션", 400,30,Rarity.Uncommom),
            new Potion("엘릭서",700,70,Rarity.Rare),
            //무기
            //검
            new Weapon("목검",300,30,WeaponType.Sword,Rarity.Common),
            new Weapon("하이랜더",1200,42,WeaponType.Sword,Rarity.Uncommom),
            new Weapon("참화도",1800,66,WeaponType.Sword,Rarity.Rare),
            //창
            new Weapon("죽창",300,29,WeaponType.Spear,Rarity.Common),
            new Weapon("나카마키",1050,40,WeaponType.Spear,Rarity.Uncommom),
            new Weapon("피나카",1900,70,WeaponType.Spear,Rarity.Rare),
            //둔기
            new Weapon("나무 망치",300,30,WeaponType.Club,Rarity.Common),
            new Weapon("타이탄",900,39,WeaponType.Club,Rarity.Uncommom),
            new Weapon("골든 스미스해머",2000,73,WeaponType.Club,Rarity.Rare),
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

        /// <summary>
        /// 각 장비 종류별 희귀도(common=0,uncommon=1,rare=2... 자세한 정보 확인은 enum Rarity 확인)에 따라 리스트에 저장
        /// Rarity추가시 반영해서 분류됨 
        /// ex. Potion[0] = common등급 Potion을 저장하는 리스트
        /// </summary>
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
                //새로운 카테고리 (ex. 유물, 악세서리 등...) 추가시 조건문 추가
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
        /// <summary>
        /// 만들어진 DB 확인
        /// </summary>
        public static void PrintResult()
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
    /*----------------------------아이템 구현부----------------------------*/
    /// <summary>
    /// 무기스킬 + 몬스터 스킬 통합
    /// </summary>
    public interface ISkill
    {
        string GetName();
        void UseSkill(Character main, Character target);
    }
    /*------------------------------캐릭터 구현부------------------------------*/
    /// <summary>
    /// 몬스터와 플레이어 클래스의 원형
    /// 몬스터의 버프 등 같은 자신, 상대 클래스 모두 적용해야해서 한개의 타입으로 묶음
    /// </summary>
    public abstract class Character
    {
        public Stat Stat { get; protected set; }
    }

    /// <summary>
    /// Character 객제의 능력치 저장
    /// </summary>
    public class Stat
    {
        public string Name { get; private set; }
        public int Hp { get; private set; }
        public int MaxHp { get; private set; }
        public int Atk { get; private set; }
        public int Def { get; private set; }
        public bool IsDead { get; private set; }
        public Stat(string name, int hp, int atk, int def)
        {
            this.Name = name;
            if (hp < 0) { hp = 100; }
            this.Hp = hp;
            this.MaxHp = hp;
            if (atk < 0) { atk = 1; }
            this.Atk = atk;
            this.Def = def;
            this.IsDead = false;
        }
        /// <summary>
        /// 체력 회복, 회복은 최대체력까지만 가능
        /// </summary>
        /// <param name="recovery">회복량</param>
        /// <returns></returns>
        public int Recovery(int recovery)
        {
            if (recovery < 0) return 0;

            if (Hp + recovery > MaxHp) recovery = MaxHp - Hp;
            Hp += recovery;
            return recovery;
        }

        /// <summary>
        /// 받는 피해량.
        /// 최소 1의 피해를 받고 체력이 0이하고 감소 시 사망(IsDead = true)
        /// </summary>
        /// <param name="damage">해당 캐릭터에 적용되는 최종 피해량</param>
        public void Damaged(int damage)
        {
            if (IsDead) { Debug.Log($"{Name}은 이미 사망했다!"); return; }
            if (damage < 0) damage = 1;
            if (damage >= Hp)
            {
                this.IsDead = true;
                Hp = 0;
            }
            else { Hp -= damage; }
            Debug.Log($"{Name}은 {damage}의 피해를 입었다! 남은 체력: {Hp}");
            if (IsDead) Debug.Log($"{Name}은 쓰러졌다!");
        }

        /// <summary>
        /// 공격력 변화량, 공격력 자체가 변하는게 아닌 change만큼 기본 공격력에서 변화
        /// 몬스터 객체 생성 시 랜덤으로 공격력 변화
        /// (공격력 최소 1 보장)
        /// </summary>
        /// <param name="change">공격력 변화량</param>
        public void ChangeAtk(int change)
        {   //buff, debuff 한번에
            //공격력이 감소하더라도 최소 1 보장
            if (Atk + change <= 0) { Atk = 1; }
            else { Atk += change; }
        }
        /// <summary>
        /// 방어력 변화량, 하한 없음(취약 개념)
        /// </summary>
        /// <param name="change">방어력 변화량</param>
        public void ChangeDef(int change)
        {
            Def += change;
        }
        /// <summary>
        /// 최대체력 변화량, 최대체력이 현재 체력보다 낮아졌을 시 최대체력으로 깎는다
        /// </summary>
        /// <param name="change">최대체력 변화량</param>
        public void ChangeMaxHp(int change)
        {
            if (MaxHp < Hp) { Hp = MaxHp; }
            else { MaxHp += change; }
        }
    }

    //*------------------------------캐릭터 기초정보 구현부------------------------------*//

    /// <summary>
    /// 플레이어가 사용하는 스킬로 ISkill 인터페이스 상속, 
    /// MonsterSkill과의 차이점으로 필요 AP가 존재
    /// </summary>
    public class PlayerSkill : ISkill
    {
        public string Name { get; protected set; }
        public int ActionPoint { get; protected set; }
        public string GetName() { return Name; }
        public virtual void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged(((Player)main).TotalAtk);
        }
    }
    public class BaseHeal : PlayerSkill
    {
        public BaseHeal()
        {
            Name = "붕대감기";
            ActionPoint = 0;
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            main.Stat.Recovery(4);
        }
    }
    public class BaseAttack : PlayerSkill
    {
        public BaseAttack()
        {
            Name = "주먹질";
            ActionPoint = 0;
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(((Player)main).TotalAtk * 0.7));
        }
    }

    public class SwardHau : PlayerSkill
    {
        public SwardHau()
        {
            Name = "썰기";
            ActionPoint = 1;
        }
    }
    public class SwardSchnitt : PlayerSkill
    {
        public SwardSchnitt()
        {
            Name = "찌르기";
            ActionPoint = 2;
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged(((Player)main).TotalAtk + ((Player)main).TotalDef);
        }
    }
    public class SwardStetchen : PlayerSkill
    {
        public SwardStetchen()
        {
            Name = "난격";
            ActionPoint = 3;
        }

        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            for (int i = 0; i < 3; i++) { target.Stat.Damaged(((Player)main).TotalAtk + UnityEngine.Random.Range(0, 5)); }
        }
    }

    public class SpearBeat : PlayerSkill
    {
        public SpearBeat()
        {
            Name = "창대치기";
            ActionPoint = 1;
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged(((Player)main).TotalAtk * 1);
        }
    }
    public class SpearCut : PlayerSkill
    {
        public SpearCut()
        {
            Name = "창날 베기";
            ActionPoint = 2;
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(((Player)main).TotalAtk * UnityEngine.Random.Range(1.5f,2.7f)));
        }
    }
    public class SpearThrust : PlayerSkill
    {
        public SpearThrust()
        {
            Name = "찌르기";
            ActionPoint = 3;
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(((Player)main).TotalAtk * UnityEngine.Random.Range(2.0f, 4.5f)));
        }
    }

    public class ClubBeat : PlayerSkill
    {
        public ClubBeat()
        {
            Name = "내려찍기";
            ActionPoint = 1;
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged(((Player)main).TotalAtk);
        }

    }
    public class ClubBbeat : PlayerSkill
    {
        public ClubBbeat()
        {
            Name = "세게 내려찍기";
            ActionPoint = 3;
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged(((Player)main).TotalAtk * 4);
        }
    }
    public class ClubBbbeat : PlayerSkill
    {
        public ClubBbbeat()
        {
            Name = "진짜세게내려찍기";
            ActionPoint = 5;
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged(((Player)main).TotalAtk * 7);
        }
    }
    //*------------------------------플레이어 스킬 구현부------------------------------*//

    /// <summary>
    /// 캐릭터의 인벤토리를 구현하는 클래스.
    /// 아이템을 추가, 삭제 크기 반환, 특정 인덱스의 장비 반환, 장비교체 기능과
    /// 인벤토리 전체 및 특정 칸 출력기능 존재.
    /// 클래스 내에서 입력값 유효성 검증
    /// </summary>
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
        {
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
        public int GetCount() { return items.Count; }
        /// <summary>
        /// 가방안의 특정 칸에 아이템을 입력
        /// </summary>
        /// <param name="idx">아이템을 넣을 인벤토리 인덱스, 해당 인덱스 내 아이템 존재시 덮어쓰기됨</param>
        /// <param name="item">인벤토리에 넣을 장비</param>
        public void Replace(int idx, Item item)
        {
            if (idx > maxSize || idx < 0)
            {
                Debug.Log($"인벤토리 Replace index Error : {idx}");
            }
            items[idx] = item;
        }

        public void Remove(int idx)
        {
            if (idx >= items.Count || idx < 0)
            {
                Debug.Log($"{idx}칸에는 장비가 없습니다 ");
            }
            else
            {
                Debug.Log($"{idx}. {items[idx].GetName()}을 제거했습니다.");
                items.RemoveAt(idx);
            }
        }
        public void Remove(int idx, bool equip)
        {
            if (idx >= items.Count || idx < 0)
            {
                Debug.Log($"{idx}칸에는 장비가 없습니다 ");
            }
            else
            {
                items.RemoveAt(idx);
            }
        }
        public Item Pick(int idx)
        {
            if (idx >= items.Count || idx < 0) return null;
            else return items[idx];
        }
        //특정 인덱스의 Item인덱스 반환
        public void PrintInventory()
        {
            Debug.Log("인벤토리를 확인합니다.\n");
            for (int i = 0; i < items.Count; i++) { items[i].PrintInfo(i); }
        }
        public void PrintInventorySlot(int idx)
        {
            if (0 <= idx && idx < items.Count) { items[idx].PrintInfo(idx); }
            else { Debug.LogWarning($"{idx}번째 칸의 장비는 없습니다. "); }
        }
    }

    /// <summary>
    /// 플레이어 클래스 
    /// 인벤토리 객체와 능력치 객체를 보유하고 장비를 착용,사용,제거,스킬사용 기능이 존재
    /// </summary>
    public class Player : Character
    {
        //구조체로 변환필요
        private int TempAtk;
        public int TotalAtk { get; private set; }
        public int atk { get; private set; }
        public int TotalDef { get; private set; }
        public int def { get; private set; }
        public int Gold { get; private set; }
        public int ActionPoint { get; set; } // 스킬 사용을 위한 비용
        Inventory inven;
        Weapon equipWeapon = null;
        Armor[] equipArmor = new Armor[] { null, null, null, null, null };
        //enum데이터 ArmorType으로 관리
        //0 -> Helmet, 1-> Armor, 2-> Glove, 3-> Gaiters, 4-> Shoes
        List<PlayerSkill>[] skillSet = new List<PlayerSkill>[(int)WeaponType.Count + 1];
        List<PlayerSkill> playerSkill;

        public Player(Stat _stat, int _maxinventorySize, int _startgold)
        {
            Stat = _stat;
            atk = Stat.Atk;
            TotalAtk = atk;
            def = Stat.Def;
            TotalDef = def;
            Gold = _startgold;
            ActionPoint = 0;
            inven = new Inventory(_maxinventorySize);
            skillSet[(int)WeaponType.Sword] = new List<PlayerSkill>
            { null, new BaseHeal(), new BaseAttack(), new SwardHau(), new SwardSchnitt(), new SwardStetchen()};
            skillSet[(int)WeaponType.Spear] = new List<PlayerSkill>
            { null, new BaseHeal(), new BaseAttack(), new SpearBeat(), new SpearCut(), new SpearThrust() };
            skillSet[(int)WeaponType.Club] = new List<PlayerSkill>
            { null, new BaseHeal(), new BaseAttack(), new ClubBeat(), new ClubBbeat(), new ClubBbbeat() };
            skillSet[(int)WeaponType.Count] = new List<PlayerSkill> //맨손(enum 카운팅변수 재활용)
            { null, new BaseHeal(), new BaseAttack()};
            playerSkill = skillSet[(int)WeaponType.Count];
        }
        public int GetMaxHp() { return Stat.MaxHp; }
        public int GetGold() { return Gold; }
        public int GetHp() { return Stat.Hp; }
        public int GetInventoryCount() { return inven.GetCount(); }
        public bool IsDead() { return Stat.IsDead; }
        public Item GetPickItem(int input) { return inven.Pick(input); }
        public void PrintInventory()
        {
            inven.PrintInventory();

        }

        public void PrintEquipItem()
        {   //장착한 장비 확인
            if (equipWeapon == null) { Debug.Log("장착 무기: 없음"); }
            else { Debug.Log($"장착 무기: {equipWeapon.GetName()}, 장비 공격력: {equipWeapon.Atk}\n총 공격력: {TotalAtk}"); }
            for (int i = 0; i < 5; i++)
            {
                if (equipArmor[i] == null) { Debug.Log($"{(ArmorType)i}부위 착용 방어구: 없음"); }
                else { Debug.Log($"{(ArmorType)i}부위 착용 방어구: {equipArmor[i].GetName()}, 장비 방어력: {equipArmor[i].Def}"); }
            }
            Debug.Log($"총 방어력: {TotalDef}");
        }
        public void PrintInfo()
        {   //캐릭터 정보 확인
            Debug.Log($"체력: {Stat.Hp}, 최대 체력 {Stat.MaxHp}, 소지금: {Gold}\n기본공격력: {Stat.Atk}, 총 공격력: {TotalAtk}, 기본방어력: {Stat.Def}, 총 방어력: {TotalDef}");
        }
        public void PrintBattleInfo()
        {
            Debug.Log($"체력: {Stat.Hp}, 최대 체력 {Stat.MaxHp}, 공격력: {TotalAtk},  총 방어력: {TotalDef} \n현재 AP: {ActionPoint}, 턴당 AP 회복량{ActionPointRegen}");

        }
        public void PrintSkill()
        {
            for (int i = 1; i < playerSkill.Count; i++)
            {
                Debug.Log($"{i}. {playerSkill[i].GetName()} ({playerSkill[i].ActionPoint})");
            }
        }
        public void UseGold(int consume) { Gold -= consume; }
        public void AddGold(int income) { Gold += income; }
        public void Attack(Monster target) { target.Hit(TotalAtk); }
        public void Hit(int damage) 
        {
            if (effect.dodge)
            {
                damage = (int)((damage-TotalDef)*0.7);
            }
            Stat.Damaged(damage);
        }
        public void Recovery(int recovery) { Stat.Recovery(recovery); }
        public void UseItem(int idx)
        {   //아이템 사용(포션/방어구/무기 분기)
            Item usingItem = inven.Pick(idx);

            if (usingItem == null)
            {
                Debug.Log($"{idx}번째 칸에는 아이템이 없습니다. ");
            }

            //선택하려고 하는 장비가 무기일 경우 실행
            if (usingItem is Weapon)
            {
                Weapon usingWeapon = (Weapon)usingItem;
                //장착한 무기가 없으면 장착하고 인벤토리에서 장착한 아이템 제거
                if (equipWeapon == null)
                {
                    equipWeapon = usingWeapon;
                    inven.Remove(idx, true);
                    Debug.Log($"{usingWeapon.GetName()} ({usingWeapon.Type})장착\n 추가 공격력: {usingWeapon.Atk}, 총 공격력: {TotalAtk}");
                }
                //장착한 무기가 있으면 장착한 무기 -> 인벤토리 후 무기 장착 
                else
                {
                    Debug.Log($"{equipWeapon.GetName()} ({equipWeapon.Type})-> ({usingWeapon.GetName()}) ({usingWeapon.Type})교체" +
                        $"\n 추가 공격력: {usingWeapon.Atk}, 총 공격력: {TotalAtk}");
                    inven.Replace(idx, equipWeapon);
                    equipWeapon = usingWeapon;
                }
                TotalAtk = equipWeapon.Atk + Stat.Atk;
                SkillUpdate();
                ActionPoint -= 1; //장비 교체 시 코스트 -1(전투 시 한정)
                if (ActionPoint < 0) ActionPoint = 0;
            }

            //선택하려고 하는 장비가 방어구인 경우 실행
            if (usingItem is Armor)
            {
                Armor usingArmor = (Armor)usingItem;
                ArmorType type = usingArmor.Type;
                //장착하려는 타입의 방어구를 장착하고있지 않을 시 장착하고 인벤에서 제거
                //장착하려는 usingItem 객체의 enum ArmorType을 정수로 캐스팅
                //0 -> Helmet, 1-> Armor, 2-> Glove, 3-> Gaiters, 4-> Shoes
                if (equipArmor[(int)type] == null)
                {
                    equipArmor[(int)type] = (Armor)usingItem;
                    Debug.Log($"{equipArmor[(int)type].GetName()} ({type})장착");
                    inven.Remove(idx, true);
                }
                //해당 타입의 방어구 장착 시 기존아이템-> 인벤, 장착하려는 아이템 장착
                else
                {
                    Debug.Log($"{equipArmor[(int)type].GetName()} ({equipArmor[(int)type].Type})-> " +
                        $"({usingArmor.GetName()}) ({usingArmor.Type})교체");

                    inven.Replace(idx, equipArmor[(int)type]);
                    equipArmor[(int)type] = (Armor)usingItem;
                }
                TotalDef = 0;
                //장착 후 총 방어력 다시 계산
                for (int i = 0; i < 5; i++)
                {
                    if (equipArmor[i] == null) continue;
                    TotalDef += equipArmor[i].Def;
                }

                TotalDef += Stat.Def;
                Debug.Log($" 추가 방어력: {usingArmor.Def}, 총 방어력: {TotalDef}");
            }
            //사용하려는 장비가 포션인 경우 사용 후 제거
            if (usingItem is Potion)
            {
                Potion usingPotion = (Potion)usingItem;
                int recovery = Stat.Recovery(usingPotion.Use());
                Debug.Log($"{usingPotion.GetName()} 사용! 회복량: {recovery}, 현재 체력: {Stat.Hp}");
                inven.Remove(idx);
            }

        }

        /// <summary>
        /// 스킬 사용
        /// </summary>
        /// <param name="idx">사용할 스킬 번호</param>
        /// <param name="target">스킬을 사용할 상대</param>
        /// <returns>상대를 공격하는데 성공 시 True, 코스트가 부족하거나 index번째 스킬이 없으면 False</returns>
        public bool CanUseSkill(int idx)
        {
            if (idx >= playerSkill.Count || idx < 0) { return false; }//없는 스킬이 들어왔을 때
            else if (ActionPoint< playerSkill[idx].ActionPoint)
            {
                Debug.Log($"코스트 부족! (보유량{ActionPoint}/요구량{playerSkill[idx].ActionPoint})");
                return false;
            }
            else 
            {
                return true;
            }
        }
        public void UseSkill(int idx, Character target)
        {
            if (effect.critical)
            {
                TempAtk = TotalAtk;
                TotalAtk = (int)(TotalAtk * 1.3);
            }
            playerSkill[idx].UseSkill(this, target);
            if(effect.critical)
            {
                TotalAtk = TempAtk;
            }
        }
        public void SkillUpdate()
        {
            if (equipWeapon == null)
            {
                playerSkill = skillSet[(int)WeaponType.Count];
            }
            else if (equipWeapon.Type == WeaponType.Sword)
            {
                playerSkill = skillSet[(int)WeaponType.Sword];

            }
            else if (equipWeapon.Type == WeaponType.Spear)
            {
                playerSkill = skillSet[(int)WeaponType.Spear];

            }
            else if (equipWeapon.Type == WeaponType.Club)
            {
                playerSkill = skillSet[(int)WeaponType.Club];
            }
        }
        public bool AddItem(Item item) { return inven.AddItem(item); }
        public void RemoveItem(int input)
        {
            Debug.Log($"{input}. {GetPickItem(input).GetName()} 제거");
            inven.Remove(input);
        }
    }
    //*------------------------------플레이어 구현부------------------------------*//
    public enum MonsterRarity { Normal, Elite, Boss }
    /// <summary>
    /// 몬스터의 최고 추상 클래스
    /// 몬스터의 정보를 출력하고 객체 생성 시 능력치가 랜덤으로 증/감해서 등장
    /// 피격판정은 가상함수로 특정몬스터는 피해량의 일부만 받는 등 오버라이딩
    /// </summary>
    public abstract class Monster : Character
    {
        //노말, 엘리트 몬스터는 2개, 보스 몬스터는 3가지의 스킬 존재
        protected MonsterRarity rarity;
        protected int rewardGold;
        public Monster()
        {
        }
        public int GetRewardGold() { return rewardGold; }
        abstract public void Initial(int stage);
        public void RandomStat()
        {
            int randAtk = UnityEngine.Random.Range(-2, 2);
            Stat.ChangeAtk(randAtk);
            int randDef = UnityEngine.Random.Range(-2, 2);
            Stat.ChangeDef(randDef);
            int randHp = UnityEngine.Random.Range(-5, 10);
            Stat.ChangeMaxHp(randHp);
        }
        abstract public void UseSkill(Character target);
        public virtual void Hit(int damage) { Stat.Damaged(damage - Stat.Def); }
        public void Print()
        {
            Debug.Log($"이름: {Stat.Name}({rarity}), 체력: {Stat.Hp}, 공격력: {Stat.Atk}, 방어력: {Stat.Def}\n");
        }
    }
    public abstract class BossMonster : Monster
    {
        public BossMonster()
        {
        }
    }
    public abstract class EliteMonster : Monster
    {
        public EliteMonster()
        {
        }

    }
    public abstract class NormalMonster : Monster
    {
        public NormalMonster()
        {
        }
    }
    //*----------------------------몬스터 구현부----------------------------*//
    enum NormalMonsterType { Slime, Skeleton, Count }
    enum EliteMonsterType { LivingArmor, Drake, Count }
    enum BossMonsterType { Dragon, Rich, Count }

    /// <summary>
    /// 입력받은 타입의 몬스터 객체 생성
    /// </summary>
    /// <typeparam name="T">생성할 객체 타입</typeparam>
    class MonsterSpawner<T> where T : Monster, new()
    {
        public static T Spawn(int stage)
        {
            T monster = new T();
            monster.Initial(stage);
            return monster;
        }
    }
    public class Dragon : BossMonster
    {
        public Dragon()
        {
        }
        public override void Initial(int stage)
        {
            this.Stat = new Stat("드래곤", 500 + (stage * 7), 30 + (stage * 2), 10 + (stage));
            this.rarity = MonsterRarity.Boss;
            this.rewardGold = 5000 + (stage * 500);
            RandomStat();
        }
        private readonly ISkill[] skill = { new DragonWing(), new DragonBreath(), new DragonWrath() };
        private readonly int[] weight = { 60, 30, 10 }; //60%확률로 기본공격, 30%확률로 강화공격, 10%확률로 특수공격
        public override void UseSkill(Character target)
        {
            int skillIdx = CalcWeight(weight);
            if (skillIdx > skill.Length || skillIdx < 0) { return; }
            else { skill[skillIdx].UseSkill(this, target); }
        }
        public override void Hit(int Damage)
        {
            base.Hit((int)(Damage * 0.75));
        }
    }
    public class Rich : BossMonster
    {
        public Rich()
        {
        }
        public override void Initial(int stage)
        {
            this.Stat = new Stat("리치", 200 + (stage * 5), 50 + (stage * 7), 5 + (int)(stage * 0.5));
            this.rarity = MonsterRarity.Boss;
            this.rewardGold = 5000 + (stage * 500);
            RandomStat();
        }
        private readonly ISkill[] skill = { new RichDeathCurrupt(), new RichIceDust(), new RichFrostChain() };
        private readonly int[] weight = { 60, 30, 10 }; //60%확률로 기본공격, 30%확률로 강화공격, 10%확률로 특수공격
        public override void UseSkill(Character target)
        {
            int skillIdx = CalcWeight(weight);
            if (skillIdx > skill.Length || skillIdx < 0) { return; }
            else { skill[skillIdx].UseSkill(this, target); }
        }
    }
    public class LivingArmor : EliteMonster
    {
        public LivingArmor()
        {
        }
        public override void Initial(int stage)
        {
            this.Stat = new Stat("리빙 아머", 70 + (stage * 3), 15 + (stage * 4), 10 + (stage * 3));
            this.rarity = MonsterRarity.Elite;
            this.rewardGold = 700 + (stage * 50);
            RandomStat();
        }
        private readonly ISkill[] skill = { new LivingArmorCut(), new LivingArmorShildBash() };
        private readonly int[] weight = { 60, 40, }; //60%확률로 기본공격, 40%확률로 강화공격
        public override void UseSkill(Character target)
        {
            int skillIdx = CalcWeight(weight);
            if (skillIdx > skill.Length || skillIdx < 0) { return; }
            else { skill[skillIdx].UseSkill(this, target); }
        }

    }
    public class Drake : EliteMonster
    {
        public Drake()
        {
        }
        public override void Initial(int stage)
        {
            this.Stat = new Stat("드레이크", 100 + (stage * 5), 20 + (stage * 5), 5 + (stage * 2));
            this.rarity = MonsterRarity.Elite;
            this.rewardGold = 700 + (stage * 50);
            RandomStat();
        }
        private readonly ISkill[] skill = { new DrakeTackle(), new DrakeBite() };
        private readonly int[] weight = { 60, 40, }; //60%확률로 기본공격, 40%확률로 강화공격
        public override void UseSkill(Character target)
        {
            int skillIdx = CalcWeight(weight);
            if (skillIdx > skill.Length || skillIdx < 0) { return; }
            else { skill[skillIdx].UseSkill(this, target); }
        }
    }
    public class Slime : NormalMonster
    {
        public Slime()
        {
        }
        public override void Initial(int stage)
        {
            this.Stat = new Stat("슬라임", 50 + (stage * 2), 5 + (stage * 4), 2);
            this.rarity = MonsterRarity.Elite;
            this.rewardGold = 300 + (stage * 40);
            RandomStat();

        }
        private readonly ISkill[] skill = { new SlimeTackle(), new SlimeSpit() };
        private readonly int[] weight = { 60, 40, }; //60%확률로 기본공격, 40%확률로 강화공격
        public override void UseSkill(Character target)
        {
            int skillIdx = CalcWeight(weight);
            if (skillIdx > skill.Length || skillIdx < 0) { return; }
            else { skill[skillIdx].UseSkill(this, target); }
        }
    }
    public class Skeleton : NormalMonster
    {
        public Skeleton()
        {
        }
        public override void Initial(int stage)
        {
            this.Stat = new Stat("각다귀", 30 + (stage * 2), 10 + (stage * 10), 0);
            this.rarity = MonsterRarity.Elite;
            this.rewardGold = 700 + (stage * 50);
            RandomStat();
        }
        private readonly ISkill[] skill = { new SkeletonPunch(), new SkeletonThrow() };
        private readonly int[] weight = { 60, 40, }; //60%확률로 기본공격, 40%확률로 강화공격
        public override void UseSkill(Character target)
        {
            int skillIdx = CalcWeight(weight);
            if (skillIdx > skill.Length || skillIdx < 0) { return; }
            else { skill[skillIdx].UseSkill(this, target); }
        }
    }
    //*-----------------------------실제 몬스터 구현부-----------------------------*//

    /// <summary>
    /// 몬스터의 스킬을 통합 관리, ISkill 인터페이스를 구현
    /// 일반, 엘리트 몬스터는 2개의 스킬, 보스몬스터는 3개의 스킬 보유
    /// </summary>
    public class MonsterSkill : ISkill
    {
        public string Name { get; protected set; }
        public string GetName() { return Name; }
        public virtual void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged(((Player)main).TotalAtk);
        }
    }
    public class DragonWing : MonsterSkill, ISkill
    {
        public DragonWing()
        {
            Name = "날개치기";
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged(main.Stat.Atk + main.Stat.Def);
        }
    }
    public class DragonBreath : MonsterSkill, ISkill
    {
        public DragonBreath()
        {
            Name = "브레스";
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(main.Stat.Atk * 1.3));
        }
    }
    public class DragonWrath : MonsterSkill, ISkill
    {
        public DragonWrath()
        {
            Name = "그냥죽어";
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(main.Stat.Atk * 2));
        }
    }
    public class RichDeathCurrupt : MonsterSkill, ISkill
    {
        public RichDeathCurrupt()
        {
            Name = "죽음과 부패";
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(main.Stat.Atk * 1.5));
        }
    }
    public class RichIceDust : MonsterSkill, ISkill
    {
        public RichIceDust()
        {
            Name = "얼음 회오리";
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(main.Stat.Atk * 2));
        }
    }
    public class RichFrostChain : MonsterSkill, ISkill
    {
        public RichFrostChain()
        {
            Name = "서리 사슬";
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(main.Stat.Atk * 3));
        }
    }

    public class LivingArmorCut : MonsterSkill, ISkill
    {
        public LivingArmorCut()
        {
            Name = "참격";
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(main.Stat.Atk + 5));
        }
    }
    public class LivingArmorShildBash : MonsterSkill, ISkill
    {
        public LivingArmorShildBash()
        {
            Name = "방패강타";
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(main.Stat.Atk + main.Stat.Def));
        }
    }

    public class DrakeTackle : MonsterSkill, ISkill
    {
        public DrakeTackle()
        {
            Name = "몸통박치기";
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(main.Stat.Atk));
        }
    }
    public class DrakeBite : MonsterSkill, ISkill
    {
        public DrakeBite()
        {
            Name = "깨물기";
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(main.Stat.Atk + UnityEngine.Random.Range(0, 21)));
        }
    }

    public class SlimeTackle : MonsterSkill, ISkill
    {
        public SlimeTackle()
        {
            Name = "몸통박치기";
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(main.Stat.Atk + 3));
        }
    }
    public class SlimeSpit : MonsterSkill, ISkill
    {
        public SlimeSpit()
        {
            Name = "점액 분사";
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(main.Stat.Atk + 3));
        }
    }

    public class SkeletonPunch : MonsterSkill, ISkill
    {
        public SkeletonPunch()
        {
            Name = "해골펀치";
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(main.Stat.Atk));
        }
    }
    public class SkeletonThrow : MonsterSkill, ISkill
    {
        public SkeletonThrow()
        {
            Name = "갈비뼈 던지기";
        }
        public override void UseSkill(Character main, Character target)
        {
            Debug.Log($"{main.Stat.Name}의 {GetName()}!");
            target.Stat.Damaged((int)(main.Stat.Atk + 3));
        }
    }
    //*------------------------------몬스터 스킬 구현부------------------------------*//
    /*-----------------------------캐릭터 구현부-----------------------------*/
    private void Start()
    {
        ItemDB.Initial();
        currentState = GameState.Start;
        Debug.Log("난이도를 선택해주세요 \n1. 겁쟁이들의 쉼터 2. 쉬움 3. 보통 4. 어려움");
    }
    /*------------------------------------FSM------------------------------------*/
    enum GameState
    {
        Start = 0,
        Main = 1,
        Inventory = 2,
        Status = 3,
        Battle = 4,
        Store = 6,
        GameEnd = 7,
        End = 8
    }
    //메인 상태 제어
    GameState currentState;
    //인벤토리 확인 상태 제어
    Player player;
    Monster monster;
    Store store;
    static bool visitedStore = false; //해당 스테이지 내 상점 방문여부
    static int potionslot = 5; //상점 포션 판매슬롯
    static int equipslot = 10;  //상점 장비 판매슬롯
    static int BaseActionPoint = 1; //전투시작 시 코스트 보유량
    static int ActionPointRegen = 2;  //매 턴 회복 코스트
    int stage = 0;

    /// <summary>
    /// 난이도 조절창. 게임시작 시 1회 출력
    /// </summary>
    public void OnStart()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            Debug.Log("난이도 선택: 겁쟁이들의 쉼터 \n게임을 시작합니다.");
            player = new Player(new Stat("Player", 200, 20, 8), 10, 5000);
            baseTickMax = 600;
            baseTickMin = 100;
            PrintMain();
            currentState = GameState.Main;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            Debug.Log("난이도 선택: 쉬움 \n게임을 시작합니다.");
            player = new Player(new Stat("Player", 200, 15, 3), 10, 1200);
            baseTickMax = 570;
            baseTickMin = 250;
            PrintMain();
            currentState = GameState.Main;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            Debug.Log("난이도 선택: 보통 \n게임을 시작합니다.");
            player = new Player(new Stat("Player", 150, 10, 1), 10, 500);
            baseTickMax = 530;
            baseTickMin = 250;
            PrintMain();
            currentState = GameState.Main;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            Debug.Log("난이도 선택: 어려움 \n게임을 시작합니다.");
            player = new Player(new Stat("Player", 100, 10, -3), 10, 500);
            baseTickMax = 380;
            baseTickMin = 200;
            PrintMain();
            currentState = GameState.Main;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            Debug.Log("테스트용.");
            player = new Player(new Stat("Player", 9999999, 1, 999999999), 10, 99999999);
            PrintMain();
            baseTickMax = 600;
            baseTickMin = 100;
            ActionPointRegen = 10;
            BaseActionPoint = 10;
            currentState = GameState.Main;
        }
    }
    /*------------------------------------FSM - 메인 상황------------------------------------*/
    public void PrintLine(string s)
    {
        Debug.Log($"/====================={s}=====================/\n");
    }
    static readonly string[] tip = new string[]
    {
        "Tip: 검은 사용한 비용에 맞는 피해량을 보장합니다 ",
        "Tip: 창은 깊게 찔리면 많이 아프지만 얕게 찔리면 큰 효과를 볼 수 없을겁니다",
        "Tip: 둔기는 느리지만 파괴력은 최고입니다",
        "Tip: 전투중에 무기 교체가 가능하지만 사용가능 코스트를 1 잃습니다",
        "Tip: 회피에 성공하면 무려 30%의 피해를 줄일 수 있습니다",
        "Tip: 적의 급소를 공격하면 30%의 추가 피해를 가할 수 있습니다 "
    };
    public void PrintTip()
    {
        Debug.Log(tip[UnityEngine.Random.Range(0, tip.Length)]);
    }
    /// <summary>
    /// 메인화면 상태 진입 시 1회 출력
    /// </summary>
    public void PrintMain()
    {
        PrintTip();
        Debug.Log("1. 상점 진입");
        Debug.Log("2. 인벤토리 열기");
        Debug.Log("3. 현재 스탯 확인");
        Debug.Log("4. 전투 입장");
    }
    /// <summary>
    /// 메인화면, 게임 중 가장 기본상태이며, 해당 상태에서 전투, 상점(스테이지 당 1번)상태로 이동가능
    /// </summary>
    public void OnMain()
    {
        //키보드 1 -> 상점
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            if (visitedStore)
            {
                Debug.Log("판매 준비중입니다.... * 전투 1회 클리어 필요");
                return;
            }
            PrintLine("Store");
            Debug.Log("상점에 진입합니다. ");
            visitedStore = true;
            currentState = GameState.Store;
            store = new Store(potionslot, equipslot);
            storeState = StoreState.Selecting;
            PrintLine("Store");
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            PrintLine("Inventory");
            player.PrintInventory();
            Debug.Log("1. 장비 사용, 2. 장비 제거                              (BackSpace:뒤로가기)");
            currentState = GameState.Inventory;
            invenState = InventoryState.Select;
            PrintLine("Inventory");
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            Debug.Log("유저 정보 확인");
            currentState = GameState.Status;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            PrintLine("Battle");
            Debug.Log("전투방 입장");
            Debug.Log("난이도 선택: 1. 일반전투, 2. 정예전투, 3. 보스전투");
            Debug.Log($"10번의 전투를 승리하면 게임을 승리합니다. 현재 승리 횟수: {stage}");
            PrintLine("Battle");
            currentState = GameState.Battle;
        }
    }

    /*------------------------------------FSM - 메인 상황------------------------------------*/

    /*------------------------------------FSM - 상점 상황------------------------------------*/
    enum StoreState { Selecting = 0, Potion = 1, Equip = 2, Sell = 3 }
    //상점 내부 상태 제어
    StoreState storeState = StoreState.Selecting;

    public void PrintRemove()
    {
        player.PrintInventory();
        Debug.Log("삭제할 인벤토리 번호 입력:        (취소: BackSpace)");
    }

    /// <summary>
    /// 상점진입 상태, 해당상태에서 상점-구매, 상점-판매 상태로 전환
    /// GameState(상점에 있음)와 StoreState(상점에서 뭔가를 함) 2개의 제어를 받음
    /// </summary>
    public void OnStore()
    {
        if (storeState == StoreState.Selecting)
        {
            //구매(1 = 포션구매, 2=장비구매) / 판매(3) 선택
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                storeState = StoreState.Potion;
                Debug.Log("포션을 구매하려고? 잘 생각했네!\n=============================== ");
                store.BuyPotionPrint();
                Debug.Log($"현재 보유 골드: {player.Gold}");

            }
            else if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                Debug.Log("장비를 구매하려고? 잘 생각했네!\n=============================== ");
                storeState = StoreState.Equip;
                store.BuyEquipPrint();
                Debug.Log($"현재 보유 골드: {player.Gold}");
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                storeState = StoreState.Sell;
                Debug.Log($"현재 보유 골드: {player.Gold}");
                store.SellItemPrint(player);
            }
            // 백스페이스 입력 시 상점에서 나와 메인화면 기능으로 전환
            else if (Input.GetKeyUp(KeyCode.Backspace))
            {
                Debug.Log("잘 가게나!");
                currentState = GameState.Main;
                PrintMain();
            }
        }
        //상점 - 포션 구매 상태
        else if (storeState == StoreState.Potion)
        {
            // 백스페이스 입력 시 상점 메인기능으로 이동
            if (Input.GetKeyUp(KeyCode.Backspace))
            {
                store.PrintInfo();
                storeState = StoreState.Selecting;
            }
            else if (Input.GetKeyUp(KeyCode.Alpha0)) { store.BuyPotion(0, player); store.BuyPotionPrint(); }
            else if (Input.GetKeyUp(KeyCode.Alpha1)) { store.BuyPotion(1, player); store.BuyPotionPrint(); }
            else if (Input.GetKeyUp(KeyCode.Alpha2)) { store.BuyPotion(2, player); store.BuyPotionPrint(); }
            else if (Input.GetKeyUp(KeyCode.Alpha3)) { store.BuyPotion(3, player); store.BuyPotionPrint(); }
            else if (Input.GetKeyUp(KeyCode.Alpha4)) { store.BuyPotion(4, player); store.BuyPotionPrint(); }
        }

        //상점 - 장비 구매 상태
        else if (storeState == StoreState.Equip)
        {
            // 백스페이스 입력 시 상점 메인기능으로 이동
            if (Input.GetKeyUp(KeyCode.Backspace))
            {
                store.PrintInfo();
                storeState = StoreState.Selecting;
            }
            else if (Input.GetKeyUp(KeyCode.Alpha0)) { store.BuyEquip(0, player); store.BuyEquipPrint(); }
            else if (Input.GetKeyUp(KeyCode.Alpha1)) { store.BuyEquip(1, player); store.BuyEquipPrint(); }
            else if (Input.GetKeyUp(KeyCode.Alpha2)) { store.BuyEquip(2, player); store.BuyEquipPrint(); }
            else if (Input.GetKeyUp(KeyCode.Alpha3)) { store.BuyEquip(3, player); store.BuyEquipPrint(); }
            else if (Input.GetKeyUp(KeyCode.Alpha4)) { store.BuyEquip(4, player); store.BuyEquipPrint(); }
            else if (Input.GetKeyUp(KeyCode.Alpha5)) { store.BuyEquip(5, player); store.BuyEquipPrint(); }
            else if (Input.GetKeyUp(KeyCode.Alpha6)) { store.BuyEquip(6, player); store.BuyEquipPrint(); }
            else if (Input.GetKeyUp(KeyCode.Alpha7)) { store.BuyEquip(7, player); store.BuyEquipPrint(); }
            else if (Input.GetKeyUp(KeyCode.Alpha8)) { store.BuyEquip(8, player); store.BuyEquipPrint(); }
            else if (Input.GetKeyUp(KeyCode.Alpha9)) { store.BuyEquip(9, player); store.BuyEquipPrint(); }
        }

        //상점 - 판매 상태
        else if (storeState == StoreState.Sell)
        {
            // 백스페이스 입력 시 상점 메인기능으로 이동
            if (Input.GetKeyUp(KeyCode.Backspace))
            {
                store.PrintInfo();
                storeState = StoreState.Selecting;
            }
            else if (Input.GetKeyUp(KeyCode.Alpha0)) { store.SellItem(0, player); store.SellItemPrint(player); }
            else if (Input.GetKeyUp(KeyCode.Alpha1)) { store.SellItem(1, player); store.SellItemPrint(player); }
            else if (Input.GetKeyUp(KeyCode.Alpha2)) { store.SellItem(2, player); store.SellItemPrint(player); }
            else if (Input.GetKeyUp(KeyCode.Alpha3)) { store.SellItem(3, player); store.SellItemPrint(player); }
            else if (Input.GetKeyUp(KeyCode.Alpha4)) { store.SellItem(4, player); store.SellItemPrint(player); }
            else if (Input.GetKeyUp(KeyCode.Alpha5)) { store.SellItem(5, player); store.SellItemPrint(player); }
            else if (Input.GetKeyUp(KeyCode.Alpha6)) { store.SellItem(6, player); store.SellItemPrint(player); }
            else if (Input.GetKeyUp(KeyCode.Alpha7)) { store.SellItem(7, player); store.SellItemPrint(player); }
            else if (Input.GetKeyUp(KeyCode.Alpha8)) { store.SellItem(8, player); store.SellItemPrint(player); }
            else if (Input.GetKeyUp(KeyCode.Alpha9)) { store.SellItem(9, player); store.SellItemPrint(player); }
        }
    }

    /*------------------------------------FSM - 상점 상황------------------------------------*/



    /*------------------------------------FSM - 인벤토리 상황------------------------------------*/
    enum InventoryState { Select = 0, Use = 1, Remove = 2 }
    InventoryState invenState = InventoryState.Select;
    /// <summary>
    /// 인벤토리 출력 및 사용안내
    /// </summary>
    public void PrintUse()
    {
        player.PrintInventory();
        Debug.Log("사용할 인벤토리 번호 입력:        (취소: BackSpace)");
    }

    /// <summary>
    /// 인벤토리 확인 상황에서 3가지 세부 상황으로 전이
    /// 1. 선택대기(ItemSelect), 2. 아이템 사용(ItemUse), 3. 아이템 제거(ItemRemove)
    /// 혹은 백스페이스를 눌러 메인 상황으로 전이
    /// </summary>
    public void OnInventory()
    {
        //인벤토리 초기화면
        if (invenState == InventoryState.Select)
        {
            InventorySelect();
        }

        //인벤토리 - 아이템 사용 상태
        else if (invenState == InventoryState.Use)
        {
            InventoryUse();
        }

        //인벤토리 - 아이템 제거 상태
        else if (invenState == InventoryState.Remove)
        {
            InventoryRemove();
        }
    }

    /// <summary>
    /// 키보드 1 입력 시 아이템 사용, 2 입력시 아이템 제거로 전이
    /// </summary>
    public void InventorySelect()
    {
        // 아이템 사용(1), 아이템 제거(2)
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            Debug.Log("아이템 사용\n");
            PrintUse();
            invenState = InventoryState.Use;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            Debug.Log("아이템 제거\n ");
            PrintRemove();
            invenState = InventoryState.Remove;
        }
        else if (Input.GetKeyUp(KeyCode.Backspace))
        {
            Debug.Log("인벤토리 닫기\n==========================================");
            PrintMain();
            currentState = GameState.Main;
        }
    }

    /// <summary>
    /// 백스페이스를 눌러 선택대기 상태로 전이하거나 0~9까지 입력받아 해당 인벤토리칸의 아이템 제거
    /// </summary>
    public void InventoryUse()
    {
        if (Input.GetKeyUp(KeyCode.Backspace))
        {
            Debug.Log("아이템 사용 종료\n==========================================");
            if (battleState == BattleState.ItemChoice)
            {
                currentState = GameState.Battle;
                battleState = BattleState.PlayerTurn;
                invenState = InventoryState.Select;
                PrintBattle();
                return;
            }
            else { invenState = InventoryState.Select; }
            player.PrintInventory();
            Debug.Log("1. 장비 사용, 2. 장비 제거                              (BackSpace:뒤로가기)");
        }
        else if (Input.GetKeyUp(KeyCode.Alpha0)) { player.UseItem(0); PrintUse(); }
        else if (Input.GetKeyUp(KeyCode.Alpha1)) { player.UseItem(1); PrintUse(); }
        else if (Input.GetKeyUp(KeyCode.Alpha2)) { player.UseItem(2); PrintUse(); }
        else if (Input.GetKeyUp(KeyCode.Alpha3)) { player.UseItem(3); PrintUse(); }
        else if (Input.GetKeyUp(KeyCode.Alpha4)) { player.UseItem(4); PrintUse(); }
        else if (Input.GetKeyUp(KeyCode.Alpha5)) { player.UseItem(5); PrintUse(); }
        else if (Input.GetKeyUp(KeyCode.Alpha6)) { player.UseItem(6); PrintUse(); }
        else if (Input.GetKeyUp(KeyCode.Alpha7)) { player.UseItem(7); PrintUse(); }
        else if (Input.GetKeyUp(KeyCode.Alpha8)) { player.UseItem(8); PrintUse(); }
        else if (Input.GetKeyUp(KeyCode.Alpha9)) { player.UseItem(9); PrintUse(); }
    }

    /// <summary>
    /// 백스페이스를 눌러 선택대기 상태로 전이하거나 0~9까지 입력받아 해당 인벤토리칸의 아이템 제거 
    /// </summary>
    public void InventoryRemove()
    {
        if (Input.GetKeyUp(KeyCode.Backspace))
        {
            Debug.Log("아이템 버리기 종료\n==========================================");
            player.PrintInventory();
            Debug.Log("1. 장비 사용, 2. 장비 제거                              (BackSpace:뒤로가기)");
            invenState = InventoryState.Select;
        }
        if (Input.GetKeyUp(KeyCode.Alpha0)) { player.RemoveItem(0); PrintRemove(); }
        if (Input.GetKeyUp(KeyCode.Alpha1)) { player.RemoveItem(1); PrintRemove(); }
        if (Input.GetKeyUp(KeyCode.Alpha2)) { player.RemoveItem(2); PrintRemove(); }
        if (Input.GetKeyUp(KeyCode.Alpha3)) { player.RemoveItem(3); PrintRemove(); }
        if (Input.GetKeyUp(KeyCode.Alpha4)) { player.RemoveItem(4); PrintRemove(); }
        if (Input.GetKeyUp(KeyCode.Alpha5)) { player.RemoveItem(5); PrintRemove(); }
        if (Input.GetKeyUp(KeyCode.Alpha6)) { player.RemoveItem(6); PrintRemove(); }
        if (Input.GetKeyUp(KeyCode.Alpha7)) { player.RemoveItem(7); PrintRemove(); }
        if (Input.GetKeyUp(KeyCode.Alpha8)) { player.RemoveItem(8); PrintRemove(); }
        if (Input.GetKeyUp(KeyCode.Alpha9)) { player.RemoveItem(9); PrintRemove(); }
    }
    /*------------------------------------FSM - 인벤토리 상황------------------------------------*/

    /*------------------------------------FSM - 스탯확인 상황------------------------------------*/

    /// <summary>
    /// 스탯 확인상태 - 장착 장비 및 스탯 확인.
    /// 1회성으로, 출력후 바로 메인화면 상태로 돌아감
    /// </summary>
    public void OnStatus()
    {
        PrintLine("Status");
        player.PrintEquipItem();
        player.PrintInfo();
        PrintLine("Status");
        PrintMain();
        currentState = GameState.Main;

    }

    /*------------------------------------FSM - 스탯확인 상황------------------------------------*/

    /*------------------------------------FSM - 전투상황------------------------------------*/
    enum BattleState { Select, Opening, PlayerTurn, UsingSkill, MonsterTurn, TurnEnd, Dodge, ItemChoice }
    BattleState battleState = BattleState.Select;
    public static class effect
    {
        public static bool critical {  get; private set; }
        public static bool dodge { get; private set; }
        public static void IsCritical(bool call)
        {
            critical = call;
        }
        public static void IsDodge(bool call)
        {
            dodge = call;
        }
    }
    int random;
    int battleTurn;
    int tick;
    int tickMax;
    int baseTickMax = 600;
    int tickMin;
    int baseTickMin = 100;
    bool foresight; //치명타
    bool dodgePhase;
    bool fastBattle = false;
    bool firstOpening = false;
    int usingSkill;
    KeyCode[] keys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    KeyCode key;
    /// <summary>
    /// 매 턴 시작이나 인벤토리 아이템 사용을 마치고 난 후 적과 자신의 상태를 출력
    /// </summary>
    public void PrintBattle()
    {
        monster.Print();
        player.PrintBattleInfo();
        player.PrintSkill();
        Debug.Log($"I. 인벤토리 아이템 사용,           - . 빠른전투 사용({fastBattle})\n=====================Turn: {battleTurn}=====================");
    }

    /// <summary>
    /// 전투 상황
    /// 세부 전투상황을 확인해 해당 상태로 전이
    /// (전투 선택, 전투 시작, 플레이어 턴, 몬스터 턴, 턴 종료 상태)
    /// </summary>
    public void OnBattle()
    {
        if (battleState == BattleState.Select)
        {
            SelectBattle();
        }
        else if (battleState == BattleState.Opening)
        {
            OpeningBattle();
        }
        else if (battleState == BattleState.PlayerTurn)
        {
            PlayerTurn();
        }
        else if (battleState == BattleState.UsingSkill)
        {
            UsingSkill();
        }
        else if (battleState == BattleState.MonsterTurn)
        {
            MonsterTurn();
        }
        else if (battleState == BattleState.TurnEnd)
        {
            EndTurn();
        }
        else if(battleState == BattleState.Dodge)
        {
            Dodge();
        }
    }

    /// <summary>
    /// 전투할 적을 결정하는 상황
    /// 1을 눌러 일반 몬스터와 전투하거나 2를 눌러 엘리트 몬스터와 전투
    /// 10번째 전투에서는 보스가 등장하며, 해당 보스를 처치하면 엔딩열람가능
    /// 전투간 플레이어 사망 시 배드엔딩 출력
    /// </summary>
    public void SelectBattle()
    {
        if (stage == 9)
        {
            Debug.Log("마지막 보스가 등장합니다!");
            random = UnityEngine.Random.Range(0, (int)BossMonsterType.Count);
            switch ((BossMonsterType)random)
            {
                case BossMonsterType.Rich:
                    { monster = MonsterSpawner<Rich>.Spawn(stage); break; }
                case BossMonsterType.Dragon:
                    { monster = MonsterSpawner<Dragon>.Spawn(stage); break; }
                default:
                    {
                        Debug.LogError("error");
                        return;
                    }
            }
            battleState = BattleState.Opening;
            return;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            random = UnityEngine.Random.Range(0, (int)NormalMonsterType.Count);
            Debug.Log("일반 전투에 진입합니다!");
            switch ((NormalMonsterType)random)
            {
                case NormalMonsterType.Slime:
                    { monster = MonsterSpawner<Slime>.Spawn(stage); break; }
                case NormalMonsterType.Skeleton:
                    { monster = MonsterSpawner<Skeleton>.Spawn(stage); break; }
                default:
                    {
                        Debug.LogError("error");
                        return;
                    }
            }
            battleState = BattleState.Opening;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            random = UnityEngine.Random.Range(0, (int)EliteMonsterType.Count);
            Debug.Log("정예 전투에 진입합니다!");
            switch ((EliteMonsterType)random)
            {
                case EliteMonsterType.LivingArmor:
                    { monster = MonsterSpawner<LivingArmor>.Spawn(stage); break; }
                case EliteMonsterType.Drake:
                    { monster = MonsterSpawner<Drake>.Spawn(stage); break; }
                default:
                    {
                        Debug.LogError("error");
                        return;
                    }
            }
            battleState = BattleState.Opening;
        }
        else if (Input.GetKeyUp(KeyCode.B))
        {
            //디버깅용
            Debug.Log("강제 보스 입장");
            stage = 9;
        }
    }

    /// <summary>
    /// 전투 시작시 한번 실행되는 상황.
    /// 턴수와 시작 코스트를 1로 초기화시키고 전투 상황창 출력
    /// 이후 플레이어 턴으로 전이됨
    /// </summary>
    public void OpeningBattle()
    {
        ///첫 오프닝때만 실행
        if(firstOpening)
        {
            battleTurn = 1;
            player.ActionPoint = BaseActionPoint;
            firstOpening = false;
        }

        dodgePhase = false;
        tick = 0;
        key = keys[UnityEngine.Random.Range(0, keys.Length)];
        //빠른전투 켜짐
        if (fastBattle)
        {
            tickMax = tickMin = 0;
        }
        //빠른전투 꺼짐
        else
        {
            tickMax = UnityEngine.Random.Range(baseTickMax-15, baseTickMax + 15);
            tickMin = UnityEngine.Random.Range(baseTickMin - 15, baseTickMin + 10);
        }
        PrintBattle();
        battleState = BattleState.PlayerTurn;
    }

    /// <summary>
    /// 플레이어 턴인 상황으로, 무기 장착 시 무기에 맞는 스킬을 사용할 수 있으며 
    /// 물약을 먹거나 장비를 교체할 수도 있음(장비 교체시 AP 1 감소).
    /// 특정 AP를 사용하여 스킬 사용가고
    /// 플레이어의 공격으로 몬스터가 사망하면 보상을 획득하고 스테이지 +1
    /// </summary>
    public void PlayerTurn()
    { //차후 구현: 전번 과제를 이용해 특정 타이밍에 랜덤한 키보드를 누르면 데미지 감소(패링)시스템 & 가하는 피해량 증가(크리티컬) 구현
      //플레이어 공격 차례
      //이 부분을 하나로 묶을 수 있는 부분이 있는지 잘 모르겠음...
        /*if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            usingSkill = 1;
            if (player.UseSkill(1, monster))
            {
                if (monster.Stat.IsDead)
                {
                    OnDead(monster);
                    return;
                }
                battleState = BattleState.MonsterTurn;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            if (player.UseSkill(2, monster))
            {
                if (monster.Stat.IsDead)
                {
                    OnDead(monster);
                    return;
                }
                battleState = BattleState.MonsterTurn;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            if (player.UseSkill(3, monster))
            {
                if (monster.Stat.IsDead)
                {
                    OnDead(monster);
                    return;
                }
                battleState = BattleState.MonsterTurn;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            if (player.UseSkill(4, monster))
            {
                if (monster.Stat.IsDead)
                {
                    OnDead(monster);
                    return;
                }
                battleState = BattleState.MonsterTurn;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            if (player.UseSkill(5, monster))
            {
                if (monster.Stat.IsDead)
                {
                    OnDead(monster);
                    return;
                }
                battleState = BattleState.MonsterTurn;
            }
        }
        */
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            player.UseSkill(1, monster);
            battleState = BattleState.MonsterTurn;
            return;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            usingSkill = 2;
            tick = 1;
            battleState = BattleState.UsingSkill;
            return;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            if (player.CanUseSkill(3))
            {
                usingSkill = 3;
                tick = 1;
                battleState = BattleState.UsingSkill;
                return;
            }

        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            if (player.CanUseSkill(4))
            {
                usingSkill = 4;
                tick = 1;
                battleState = BattleState.UsingSkill;
                return;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            if (player.CanUseSkill(5))
            {
                usingSkill = 5;
                tick = 1;
                battleState = BattleState.UsingSkill;
                return;
            }
        }
        else if (Input.GetKeyUp(KeyCode.I))
        {
            Debug.Log("아이템 사용\n");
            PrintUse();
            battleState = BattleState.ItemChoice;
            currentState = GameState.Inventory;
            invenState = InventoryState.Use;
        }
        else if (Input.GetKeyUp(KeyCode.Minus))
        {
            fastBattle = !fastBattle;
            if (fastBattle)
            {
                Debug.Log("빠른 전투 활성화!");
                battleState = BattleState.Opening;
            }
            else
            {
                Debug.Log("빠른 전투 비활성화!");
                battleState = BattleState.Opening;
            }
        }
    }
    public void UsingSkill()
    {
        tick += 1;

        if (tick == tickMin)
        {
            switch (key)
            {
                case KeyCode.W:
                    {
                        Debug.Log("적의 머리의 급소를 찾았습니다! \n(Press W)");
                        return;
                    }
                case KeyCode.A:
                    {
                        Debug.Log("적의 왼팔에 약점을 찾았습니다! \n(Press A)");
                        return;
                    }
                case KeyCode.S:
                    {
                        Debug.Log("적의 다리에 약점을 찾았습니다! \n(Press S)");
                        return;
                    }
                case KeyCode.D:
                    {
                        Debug.Log("적의 오른팔에 약점을 찾았습니다! \n(Press D)");
                        return;
                    }
                default:
                    {
                        Debug.LogWarning("구문오류");
                        return;
                    }
            }
        }
        if (effect.critical)
        {
            Debug.Log("와...너 정말, **약점** 을 찔렀어... 30% 추가 피해!");
            player.UseSkill(usingSkill, monster);
            effect.IsCritical(false);
            if (monster.Stat.IsDead)
            {
                OnDead(monster);
                return;
            }
            battleState = BattleState.MonsterTurn;
            Debug.Log("몬스터의 공격 차례! ");

        }
        if (tickMin < tick && tick < tickMax)
        {
            switch (key)
            {
                case KeyCode.W:
                    {
                        if (Input.GetKeyDown(KeyCode.W)) { effect.IsCritical(true); }
                        return;
                    }
                case KeyCode.A:
                    {
                        if (Input.GetKeyDown(KeyCode.A)) { effect.IsCritical(true); }
                        return;
                    }
                case KeyCode.S:
                    {
                        if (Input.GetKeyDown(KeyCode.S)) { effect.IsCritical(true); }
                        return;
                    }
                case KeyCode.D:
                    {
                        if (Input.GetKeyDown(KeyCode.D)) { effect.IsCritical(true); }
                        return;
                    }
                default:
                    {
                        Debug.LogWarning("구문오류");
                        return;
                    }
            }

        }
        else if (tick > tickMax && !foresight)
        {
            Debug.Log("약점 타격에 실패했습니다...");
            player.UseSkill(usingSkill, monster);

            if (monster.Stat.IsDead)
            {
                OnDead(monster);
                return;
            }
            battleState = BattleState.MonsterTurn;
            Debug.Log("몬스터의 공격 차례! ");

        }
    }
    
    public void Dodge()
    {
        tick += 1;

        if (tick == tickMin)
        {
            switch (key)
            {
                case KeyCode.W:
                    {
                        Debug.Log("적이 머리를 공격하려고 합니다! \n(Press W)");
                        return;
                    }
                case KeyCode.A:
                    {
                        Debug.Log("적이 오른쪽에서 공격하고 있습니다! \n(Press A)");
                        return;
                    }
                case KeyCode.S:
                    {
                        Debug.Log("적이 돌진합니다! \n(Press S)");
                        return;
                    }
                case KeyCode.D:
                    {
                        Debug.Log("적의 왼쪽에서 공격하고 있습니다! \n(Press D)");
                        return;
                    }
                default:
                    {
                        Debug.LogWarning("구문오류");
                        return;
                    }
            }
        }
        if (effect.dodge)
        {
            Debug.Log("회피에 성공했습니다! 30% 피해 감소!...");
            dodgePhase = true;
            battleState = BattleState.MonsterTurn;
        }
        if (tickMin < tick && tick < tickMax)
        {
            switch (key)
            {
                case KeyCode.W:
                    {
                        if (Input.GetKeyDown(KeyCode.W)) { effect.IsDodge(true); }
                        return;
                    }
                case KeyCode.A:
                    {
                        if (Input.GetKeyDown(KeyCode.A)) { effect.IsDodge(true); }
                        return;
                    }
                case KeyCode.S:
                    {
                        if (Input.GetKeyDown(KeyCode.S)) { effect.IsDodge(true); }
                        return;
                    }
                case KeyCode.D:
                    {
                        if (Input.GetKeyDown(KeyCode.D)) { effect.IsDodge(true); }
                        return;
                    }
                default:
                    {
                        Debug.LogWarning("구문오류");
                        return;
                    }
            }

        }
        else if (tick > tickMax && !foresight)
        {
            Debug.Log("회피에... 실패했습니다...");
            dodgePhase = true;
            battleState = BattleState.MonsterTurn;
        }
    } 
    /// <summary>
    /// 몬스터가 공격하고, 해당 공격으로 플레이어 사망 시 엔딩 출력
    /// </summary>
    public void MonsterTurn()
    {
        key = keys[UnityEngine.Random.Range(0, keys.Length)];
        tick = 1;

        //회피 판정(Dodge())을 시도하지 않았으면 회피 시도
        if (!dodgePhase) 
        {
            battleState = BattleState.Dodge;
            return;
        }

        monster.UseSkill(player);
        effect.IsDodge(false);

        if (player.IsDead())
        {
            OnDead(player);
            return;
        }
        battleState = BattleState.TurnEnd;
    }

    /// <summary>
    /// 플레이어와 몬스터의 턴이 종료되었을 시 발생하는 턴으로
    /// 상태이상과 같은 특수효과를 갱신하고 해당효과로 적이 사망했는지 확인
    /// (미구현)
    /// </summary>
    public void EndTurn()
    {
        //기타 디버프, 도트딜 구현

        //둘다 동시에 사망 시 게임오버
        if (player.Stat.IsDead)
        {
            OnDead(player);
            return;
        }
        else if (monster.Stat.IsDead)
        {
            OnDead(monster);
            return;
        }
        player.ActionPoint += ActionPointRegen;
        battleTurn++;
        battleState = BattleState.Opening;
    }

    /// <summary>
    /// target의 사망여부 확인
    /// </summary>
    /// <param name="target">확인할 대상</param>
    public void OnDead(Character target)
    {
        firstOpening = true;
        if (target is Monster)
        {
            Debug.Log($"전투에서 승리했다! Player는 {monster.GetRewardGold()}Gold를 획득했다!" +
                $"\n==========================================");
            player.AddGold(monster.GetRewardGold());

            //최종보스 처치 시 게임 승리(종료)
            if (stage == 9)
            {
                currentState = GameState.GameEnd;
                return;
            }
            //stage 클리어 횟수 추가
            else
            {
                stage++;
                PrintMain();
                battleState = BattleState.Select;
                currentState = GameState.Main;
                visitedStore = false;
                return;
            }
        }
        else if (target is Player)
        {
            Debug.Log("전투에서 패배했다!!");
            currentState = GameState.GameEnd;
            return;
        }
    }
    /*------------------------------------FSM - 전투상황------------------------------------*/

    /*------------------------------------FSM - 엔딩------------------------------------*/

    /// <summary>
    /// 플레이어가 사망 상태로 진입 시 BadEnding
    /// 플레이어 생존 상태로 진입 시 GoodEnding
    /// </summary>
    public void Ending()
    {

        if (player.IsDead())
        {
            Debug.Log("당신은 이 모험의 끝을 볼 수 없었습니다...");
        }
        else
        {
            Debug.Log("당신은 마침내 모든 적을 해치웠습니다! ");
        }
        currentState = GameState.End;
    }


    /*------------------------------------FSM - 엔딩------------------------------------*/

    /*------------------------------------FSM - 실행부------------------------------------*/
    /// <summary>
    /// GameState상태에 따른 전이
    /// </summary>
    private void Update()
    {
        if (currentState == GameState.Start)
        {
            OnStart();
        }
        else if (currentState == GameState.Main)
        {
            OnMain();
        }
        else if (currentState == GameState.Store)
        {
            OnStore();
        }
        else if (currentState == GameState.Inventory)
        {
            OnInventory();
        }
        else if (currentState == GameState.Status)
        {
            OnStatus();
        }
        else if (currentState == GameState.Battle)
        {
            OnBattle();
        }
        else if (currentState == GameState.GameEnd)
        {
            Ending();
        }
    }
    /*------------------------------------FSM - 실행부------------------------------------*/
    /*------------------------------------FSM------------------------------------*/
}


/*
최종 과제 구현목표
1. 게임 타입 - 슬더슬구조, 전투는 림버스+쯔꾸르 -> 일요일 추가
2. 게임 내부 
2-1. 시작: 캐릭터 시작 선택지 구현(고래) -> 실패! 
2-2. 선택지:  일반전투, 엘리트 전투, 이벤트, 상점, 휴식 5가지 -> 구조 변경(전투, 인벤토리, 상태창, 상점)
2-3. 전투구조: 일반몬스터와 엘리트 몬스터가 존재하고 ,각 전투 조우는 프리셋된 몬스터 묶음에서 랜덤으로 조우 ->우선적으로 고정해서 구현, 일요일 변경
2-4. 전투구조: 공격 타입은 3가지로, 각 적마다 내성이 존재, 전투진입 전 어떤 적들이 나올지 알려주고 장비를 바꿀 기회를 줌  -> 구현 X
2-5. 이벤트구조: (강화,제거,변화) 3가지 + 특수이벤트(특정 카드/골드 소모해 보상획득 / 선택지 골라서 운에따라 결과발생) -> 일요일에 추가
2-6. 스테이지: 이번주 과제에서는 일단 1스테이지만 만들고 8번의 경로를 선택한 후 상점 -> 휴식고정 -> 보스 -> 일요일 추가
오늘 과제 구현목표
1. 상점 들어가기 , 아이템 구매 V
1-1. 아이템은 3가지 (무기, 갑옷, 물약) V
1-2. 무기는 검(참격), 창(관통), 둔기(타격) V
1-3. 각 무기마다 3개의 스킬(스킬1 -> 약한 공격 / 스킬2 -> 방어 / 스킬3 -> 강한 공격) V
금요일 과제 구현목표
1. 목요일 만든 코드 FMS로 변환 V
2. Player 클래스 최적화 ??
3. 전투 시스템 구현 (각 전투는 1-1) V
4. 전투 시스템 FMS로 변환 V
주말 내 과제 구현목표
1. 랜덤 분기 그리드 생성(이벤트,일반전투,정예전투,상점,휴식) -> 제한을 통해 예상외의 구조(연속 2번 상점, 1스테이지부터 정예전투...) 제외 -> 일요일
2. stage 1 구현(8개 맵 -> 상점 -> 휴식 -> 보스전투) -> X
2-1. 각 전투마다 1-다수 전투 구현 -> X
3. 장비 강화 구현 ->구현 X
4. 레벨업에 따른 스펙업 디자인 계획 -> 일요일 
5. 난이도 디자인 공부 -> 일요일
6. 다양한 이벤트 구현(객체로? 함수로?) -> 일요일 
7. 엔딩 제작 V
 */
