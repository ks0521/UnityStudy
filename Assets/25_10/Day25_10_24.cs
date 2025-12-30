using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day25_10_24 : MonoBehaviour
{
    /// <summary>
    /// 장비를 구매하는 함수
    /// </summary>
    /// <param Name="UserGold">유저가 보유하고 있는 골드</param>
    /// <param Name="Store">상점 배열</param>
    /// <param Name="ItemInfo">아이템의 이름</param>
    /// <param Name="x">상점에서 구매하려는 물품의 행</param>
    /// <param Name="y">상점에서 구매하려는 물품의 열</param>
    /// <param Name="UserInventory">유저의 인벤토리</param>
    /// <returns></returns>
    bool BuyItem(ref int UserGold,in int[,]Store,in string[] ItemInfo,int x,int y,int[,]UserInventory)
    {
        int row = Store.GetLength(0);
        int col = Store.GetLength(1);
        int EmptyX, EmptyY;
        
        if (x < 0 || x >= row || y < 0 || y >= col) 
        { //상점 배열 범위를 초과한 경우
            Debug.LogFormat("잘못된 상점 좌표 : [{0},{1}]", x, y);
            return false;
        }
        int index = x * col + y + 1;
        if (index < 1 || index >= ItemInfo.Length)
        { //ItemInfo 범위를 벗어난 경우(아이템 코드는 1~ItemInfo.Length-1)
            Debug.LogWarning("Invalid value!");
            return false;
        }
        int Price = Store[x, y]; //사려고 하는 물건의 가격
        if (Price > UserGold)
        { //장비를 구매할 돈이 없는 경우
            Debug.LogFormat("{0}을(를) 구매할 골드가 부족합니다!\n 구매 필요 골드: {1}골드", ItemInfo[index], Price);
            return false;
        }
        if (!TryFindEmptySlot(UserInventory,out EmptyX,out EmptyY))
        { //인벤토리에 빈 칸이 없는 경우 false, 아니면 인벤토리의 비어있는 좌표 출력
            Debug.Log("인벤토리에 빈 칸이 없습니다!\n");
            return false;
        }
        //아이템 정보가 있는 경우 (1 ~ ItemInfo.length)
        Debug.LogFormat("현재 {0}골드 보유\n", UserGold);
        UserGold -= Price;
        Debug.LogFormat("{0}구매 성공({1}Gold 지불) : {2}Gold 남음\n", ItemInfo[index], Price, UserGold);
        InputInventory(UserInventory, index, EmptyX,EmptyY);
        return true;
    }

    /// <summary>
    /// 장비를 인벤토리의 공간에 넣는다
    /// </summary>
    /// <param Name="UserInventory">유저 인벤토리</param>
    /// <param Name="PurchaseItem">구매한 장비의 인덱스</param>
    /// <param Name="EmptyX">넣을 인벤토리의 행</param>
    /// <param Name="EmptyY">넣을 인벤토리의 열</param>
    void InputInventory(int[,] UserInventory, int PurchaseItem,int EmptyX, int EmptyY)
    { //한줄이지만 이후 사냥, 거래, ... 아이템 획득 로직구현을 위해 함수제작
        UserInventory[EmptyX,EmptyY] = PurchaseItem;
    }

    //해당 인벤토리가 꽉 찼는지 확인
    //비어있으면 true, 꽉 찼으면 false
    bool TryFindEmptySlot(in int[,] UserInventory,out int x,out int y)
    {
        int row = UserInventory.GetLength(0);
        int col = UserInventory.GetLength(1);

        for (int i = 0; i < row; i++) 
        {
            for (int j = 0; j < col; j++)
            {
                if (UserInventory[i, j] == 0)
                {
                    x = i;
                    y = j;
                    return true;
                }
            }
        }
        x = -1;
        y = -1;
        return false;
    }

    /// <summary>
    /// 인벤토리에서 장비를 장착하는 함수
    /// </summary>
    /// <param Name="UserAbility">현재 유저의 전투력</param>
    /// <param Name="index">장착하려는 특정 인벤토리 변수의 참조</param>
    /// <param Name="ItemIndex">사용하려는 아이템의 정보</param>
    void UseItem(ref float UserAbility, ref int index, in string[] ItemIndex)
    {
        float ItemAbility = 0;
        switch (index)
        {
            case 1: ItemAbility = 9f; break; //목검
            case 2: ItemAbility = 13.5f; break; //철검
            case 3: ItemAbility = 20f; break; //다마스커스
            case 4: ItemAbility = 5f; break; //나무방패
            case 5: ItemAbility = 7.3f; break; //철방패
            case 6: ItemAbility = 9.7f; break; //등패
            case 7: ItemAbility = 10.5f; break; //두정갑
            case 8: ItemAbility = 13f; break; //판금갑옷
            case 9: ItemAbility = 17.5f; break; //뼈갑옷
            default: return;
        }

        UserAbility += ItemAbility;
        Debug.LogFormat("{0} 장착: 전투력 {1}상승 \n 총 전투력: {2}", ItemIndex[index], ItemAbility, UserAbility);
        index = 0;

    }

    /// <summary>
    /// 인벤토리에 있는 물약을 사용하는 함수
    /// </summary>
    /// <param Name="UserHP">유저의 현재 HP</param>
    /// <param Name="MaxHP">캐릭터의 최대 HP</param>
    /// <param Name="index">사용하려는 특정 인벤토리 변수의 참조</param>
    /// <param Name="ItemIndex">사용하려는 아이템의 정보</param>
    void UseItem(ref int UserHP, int MaxHP, ref int index, in string [] ItemIndex)
    {
        int ItemHP = 0;
        
        switch (index)
        {
            case 10: ItemHP = 30; break; //약초
            case 11: ItemHP = 50; break; //포션
            case 12: ItemHP = 90; break; //엘릭서
            default: return;
        }

        if (ItemHP + UserHP > MaxHP)
        {//최대체력 초과시 최대체력까지만 회복
            ItemHP = MaxHP - UserHP;
            UserHP = MaxHP;
        }
        else
        {
            UserHP += ItemHP;
        }

        Debug.LogFormat("{0} 사용! 체력 {1} 회복 -> 현재 체력: {2}\n", ItemIndex[index], ItemHP, UserHP);
        index = 0;
    }

    /// <summary>
    /// 백업본 저장
    /// </summary>
    /// <param Name="Original">백업을 저장할 원본 인벤토리</param>
    /// <param Name="Backup">백업 인벤토리</param>
    void SaveInventory(in int[,]Original, int[,]Backup)
    { //save = BackupInventory <- UserInventory
        int row = Original.GetLength(0);
        int col = Original.GetLength(1);

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++) 
            {
                Backup[i,j]=Original[i,j];
            }
        }
    }

    //원본과 백업본 순서만 바꿔서 SaveInventory 함수로 전달
    void LoadInventory(int[,] Original,in int[,] Backup)
    { //Load = SaveInventory의 역(UserInventory <- BackupInventory)
        SaveInventory(Backup, Original);
    }

    //인벤토리 내 아이템 출력
    void PrintInventory(in int[,] inventory, in string[] ItemInfo)
    {
        for (int i = 0; i < inventory.GetLength(0); i++)
        {
            for (int j = 0; j < inventory.GetLength(1); j++)
            {
                if (inventory[i, j] < 0 || inventory[i, j] > ItemInfo.Length - 1) 
                { //인벤토리 내에 있는 정보가 오류가 났을 시 출력
                    Debug.LogFormat("[{0},{1}] Invalid : {2}", i, j, inventory[i, j]);
                    continue;
                }
                Debug.LogFormat("[{0}, {1}] -> {2}\n", i, j, ItemInfo[inventory[i, j]]);
            }
        }
    }

    bool SortInventory(int[,] UserInventory, int MaxIndex, int[,] BackupInventory)
    {
        int[] CountingSort = new int[MaxIndex + 1];
        int row = UserInventory.GetLength(0);
        int col = UserInventory.GetLength(1);
        SaveInventory(in UserInventory, BackupInventory);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (UserInventory[i, j] > MaxIndex || UserInventory[i, j] < 0)
                { //아이템 인덱스 범위 오류 탐지
                    Debug.LogFormat("[{0},{1}] Invalid : {2}", i, j, UserInventory[i, j]);
                    //오류 발생 시 백업본 덮어쓰기 (UserInventory 무결성 훼손)
                    LoadInventory(UserInventory, BackupInventory);
                    return false;
                }
                CountingSort[UserInventory[i, j]]++;
                //인벤토리에 있는 아이템의 인덱스 ++
                //ex. UserInventory[i,j] = 1 (목검)이면 CountingSort[1]++
                UserInventory[i, j] = 0;
                //정렬 후 삽입과정에서 빈칸을 만들기위해 0으로 초기화
            }
        }

        int Index = 1;
        while (Index <= MaxIndex && CountingSort[Index] <= 0)
        { //가장 작은 인덱스의 값이 0이 아닐때까지(=Index번 장비가 존재할때까지) +1
          // 반드시 좌항에 Index <= MaxIndex가 먼저 와야함
            Index++;
        }

        for (int i = 0; i < row && Index <= MaxIndex; i++)
        { //index>maxindex = 정렬 전 인벤토리에 있는 장비는 다 넣었다!  
            for (int j = 0; j < col && Index <= MaxIndex; j++)
            {
                UserInventory[i, j] = Index;
                CountingSort[Index] -= 1;
                while (Index <= MaxIndex && CountingSort[Index] <= 0)
                {//상단의 While문과 동일한 목적
                    Index++;
                }
            }
        }
        return true;
    }
    void Start()
    { //장비 구매, 착용을 입출력으로 구현하고자 대부분의 조건을 변수로 작성하고 특정 부분만 변수에 대입하는 식으로 진행했습니다
        string[] ItemInfo = new string[]
        {"비어있음", "목검", "철검", "다마스커스", "나무방패", "철방패", "등패", "두정갑", "판금갑옷", "뼈갑옷", "약초", "포션", "엘릭서"};
        //ItemInfo 인덱스가 아이템 코드(ex. ItemInfo[1]="철검") , 인덱스는 1부터 시작(0은 비어있음)
        int[,] Store = new int[4, 3]
        { //행 인덱스*3+열 인덱스+1 -> 아이템 코드(ex. Store[1,1] = 1*3 + 1 + 1 = 5 -> ItemInfo[5]="철방패")
            //+1한 이유는 0번은 비어있음으로 구현해야 하기 때문
            {200, 500, 4000 },
            {120, 300, 250 },
            {500, 750, 500 },
            {30, 50, 70 }
        };
        float UserAbility = 10;
        int UserHp = 35;
        int MaxHp = 100;
        int UserGold = 7500;
        int col = Store.GetLength(1); //상점의 열 (인덱싱 위해서 지정)
        int[,] UserInventory = new int [2, 2];
        //인벤토리 내부 값은 비어있으면 0, 장비가 있자면 해당 장비의 인덱스
        int[,] BackupInventory = new int[UserInventory.GetLength(0), UserInventory.GetLength(1)];
        //내 인벤토리와 같은 크기의 백업용 인벤토리 생성

        int PurchaseItemX=1, PurchaseItemY=1;
        BuyItem(ref UserGold, Store, ItemInfo, PurchaseItemX, PurchaseItemY, UserInventory);

        PurchaseItemX = 0; PurchaseItemY = 2;
        BuyItem(ref UserGold, Store, ItemInfo, PurchaseItemX, PurchaseItemY, UserInventory);


        PurchaseItemX = 3; PurchaseItemY = 2;
        BuyItem(ref UserGold, Store, ItemInfo, PurchaseItemX, PurchaseItemY, UserInventory);
        BuyItem(ref UserGold, Store, ItemInfo, PurchaseItemX, PurchaseItemY, UserInventory);
        BuyItem(ref UserGold, Store, ItemInfo, PurchaseItemX, PurchaseItemY, UserInventory);

        Debug.Log("현재 인벤토리 출력\n");
        PrintInventory(UserInventory,ItemInfo);

        for (int i = 0; i < UserInventory.GetLength(0); i++)
        { 
            for(int j = 0; j < UserInventory.GetLength(1); j++)
            { //사용하면 사라짐
                if (UserInventory[i, j] > 0 && UserInventory[i, j] <= 9) 
                { //장비 장착
                    UseItem(ref UserAbility, ref UserInventory[i,j],  ItemInfo);
                }
                else if (UserInventory[i, j] >= 10 && UserInventory[i, j] <= 12) 
                { //물약 섭취
                    UseItem(ref UserHp, MaxHp, ref UserInventory[i,j], ItemInfo);
                }
            }
        }
        UserInventory[0, 0] = 12;
        UserInventory[0, 1] = 3;
        UserInventory[1, 0] = 5;
        //UserInventory[1, 1] = 12;


        //인벤토리를 백업
        Debug.Log("원본  \n");
        PrintInventory(UserInventory, ItemInfo);
        SaveInventory(UserInventory, BackupInventory);

        //백업한 인벤토리를 출력
        Debug.Log("저장본 \n");
        PrintInventory(BackupInventory, ItemInfo);

        //오류 발생을 가정하고 현재 인벤토리를 초기화
        for (int i = 0; i < UserInventory.GetLength(0); i++)
        { // 백업 확인을 위한 원본 인벤토리 제거
            for (int j = 0; j < UserInventory.GetLength(1); j++)
            {
                UserInventory[i,j] = 0;
            }
        }
        Debug.Log("삭제 후 \n");
        PrintInventory(UserInventory, ItemInfo);
        //초기화한 인벤토리를 백업
        LoadInventory(UserInventory, BackupInventory);
        Debug.Log("불러오기 후 \n");
        PrintInventory(UserInventory, ItemInfo);
        int MaxIndex = ItemInfo.Length - 1;
        if (SortInventory(UserInventory, MaxIndex, BackupInventory))
        {
            Debug.Log("정렬 성공! ");
            PrintInventory(UserInventory, ItemInfo);
        }

    }
}
