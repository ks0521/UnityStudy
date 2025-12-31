using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Day25_10_23 : MonoBehaviour
{
    void ShowInventory(int[]inventory, string[] Items)
    {
        int item;
        for (int i = 0; i < inventory.Length; i++)
        {
            PrintLine(i+1);
            item = inventory[i];
            if(item>0 && item <= 6)
            {
                Debug.LogFormat("{0}을 가지고 있습니다. ", Items[item]);
            }
            else
            {
                Debug.LogWarning("혀용되지 않은 접근입니다. ");
            }
        }
    }
    
    void PrintLine(int count = 1)
    {
        Debug.LogFormat("-------{0}번째 탐색-------",count);
    }

    bool UsingItem(ref float UserPower, int[,] inventory,string[] items,int x,int y)
    { //장비 사용 시 실행
        if (x > inventory.GetLength(0) || x < 0 || y > inventory.GetLength(1) || y < 0)
        { //out of range
            return false;
        }

        Debug.LogFormat("장비 사용 전 전투력: {0}", UserPower);
        int item = inventory[x,y];
        if (item == 1) //목검 사용
        {
            Debug.LogFormat("{0} 사용", items[item]);
            UserPower += 2.5f;
        }
        else if (item == 2) //철검 사용
        {
            Debug.LogFormat("{0} 사용", items[item]);
            UserPower += 5f;
        }
        else if (item == 5) //나무방패 사용
        {
            Debug.LogFormat("{0} 사용", items[item]);
            UserPower += 1.5f;
        }
        else if(item == 6) //철제방패 사용
        {
            Debug.LogFormat("{0} 사용", items[item]);
            UserPower += 3f;
        }
        else
        {
            return false;
        }
        return true;
    }
    bool UsingItem(ref int UserHP, int[,]inventory,string [] items,int x,int y)
    { //물약 사용 시 실행
        if (x > inventory.GetLength(0) || x < 0 || y > inventory.GetLength(1) || y < 0)
        { //out of range
            return false;
        }

        Debug.LogFormat("장비 사용 전 체력: {0}", UserHP);
        int item = inventory[x,y];
        if (item == 3)
        {
            Debug.LogFormat("{0} 사용", items[item]);
            UserHP += 50;
        }
        else if( item == 4)
        {
            Debug.LogFormat("{0} 사용", items[item]);
            UserHP += 250;
        }
        else //Items 인덱스를 벗어나거나 장비를 사용한 경우
        {
            return false;
        }
        return true;

    }
    void SaveItem(int[,] Inventory, int[,] SaveInventory)
    {
        int x = Inventory.GetLength(0);
        int y = Inventory.GetLength(1);
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                SaveInventory[i,j]=Inventory[i,j];
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        /*
        1: 목검 / 2: 철검 / 3: 맑은 물 / 4: 엘릭서 / 5: 목재 방패 / 6: 철제 방패 
        */
        int UserHP=70;
        float UserPower=52.5f;
        int[] Inventory = new int[3];
        int[,] AdvInventory = new int[3, 2] { { 1, 2 }, { 6, 5 }, { 4, 3 } };
        int[,] SavedInventory;
        string[] Items = new string[7] {"", "목검", "철검", "맑은 물", "엘릭서", "목재 방패", "철제 방패" };
        Inventory[0] = 1;
        Inventory[1] = 2;
        Inventory[2] = 4;

        //과제 1 + 과제 3
        //인벤토리 장비 출력 + 대화라인 구분
        for(int i = 0; i < 3; i++)
        {
            PrintLine(i+1);
            if (Inventory[i] > 0 && Inventory[i] <= 6 )
            {
                Debug.LogFormat("{0}을 가지고 있습니다. ", Items[Inventory[i]]);
            }
            else
            { //인벤토리에 있는 장비의 인덱스가 Items에 존재하지 않는 인덱스인경우
                Debug.LogWarning("허용되지 않은 접근입니다. ");
            }
        }

        //과제 2
        //과제 1+3 함수화
        ShowInventory(Inventory,Items);

        //과제 4
        //아이템 사용 함수
        //장비(검, 방패) 장착 시 UserPower, 음식 섭취 시 UserHP에 영향을 줌
        if(UsingItem(ref UserPower, AdvInventory, Items, 0, 1))
        { //장비 장착
            Debug.LogFormat("장비 사용 후 전투력: {0}", UserPower);
        }
        else //item 인덱스를 벗어나거나 물약 인덱스에 접근한 경우
        {
            Debug.LogWarning("잘못된 장비 접근"); 
        }
        if (UsingItem(ref UserPower, AdvInventory, Items, 2, 1))
        { //오류 발생 예시(아이템 종류 오류)
            Debug.LogFormat("장비 사용 후 전투력: {0}", UserPower);
        }
        else //item 인덱스를 벗어나거나 물약 인덱스에 접근한 경우
        {
            Debug.LogWarning("잘못된 장비 인덱스 접근");
        }

        if (UsingItem(ref UserHP, AdvInventory, Items, 2, 0))
        { //음식 섭취
            Debug.LogFormat("음식 섭취 후 체력: {0}", UserHP);
        }
        else //item 인덱스를 벗어나거나 장비 인덱스에 접근한 경우
        {
            Debug.LogWarning("잘못된 장비 인덱스 접근");
        }
        if (UsingItem(ref UserHP, AdvInventory, Items, 2, 5))
        { //오류 발생 예시(out of range)
            Debug.LogFormat("음식 섭취 후 체력: {0}", UserHP);
        }
        else //item 인덱스를 벗어나거나 장비 인덱스에 접근한 경우
        {
            Debug.LogWarning("잘못된 장비 인덱스 접근");
        }
        //과제 5
        //인벤토리 저장
        int x = AdvInventory.GetLength(0);
        int y = AdvInventory.GetLength(1);
        SavedInventory = new int[x, y];
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            { //기존 배낭에 들어있는 아이템 출력
                Debug.LogFormat("기존 {0},{1} = {2}",i,j,AdvInventory[i,j]);
            }
        }
        SaveItem(AdvInventory,SavedInventory);
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            { //저장한 배낭에 들어있는 아이템 출력
                Debug.LogFormat("저장된 {0},{1} = {2}", i, j, AdvInventory[i, j]);
            }
        }
    }
}
