using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class quiz : MonoBehaviour
{
    int[,] map = new int[6, 9]
        { //0: 갈 수 없는길, 1: 갈 수 있는 길
        {1,1,0,0,0,0,0,0,0 },
        {1,1,1,0,0,0,0,0,0 },
        {0,1,1,1,1,1,0,0,0 },
        {0,1,1,1,1,1,0,0,0 },
        {0,0,0,0,1,1,1,1,1 },
        {0,0,0,0,0,1,1,1,1 },
        };
    int PositionX = 0; //map[x,y] 기준
    int PositionY = 0;
    int[] dx = new int[4] { 0, 1, 0, -1 };
    int[] dy = new int[4] { 1, 0, -1, 0 };

    bool TryMovePosition(int flag)
    {
        int Xlength = map.GetLength(0);
        int Ylength = map.GetLength(1);
        int NextX = PositionX + dx[flag];
        int NextY = PositionY + dy[flag];

        if (NextX < 0 || NextY < 0 || NextX >= Xlength || NextY >= Ylength)
        {   //out of range 검사
            Debug.LogWarning("out of range");
            return false;
        }
        if (map[NextX, NextY] == 0)
        {   //갈 수 있는 길인지 검사 
            Debug.LogWarning("넌 못지나간다");
            return false;
        }
        Debug.LogFormat("({0},{1}) to ({2},{3})", PositionX, PositionY, NextX, NextY);
        PositionX = NextX;
        PositionY = NextY;

        return true;
    }
    void Start()
    {
        
        
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.D)) //(0,+1)
        {
            TryMovePosition(0);
        }
        if (Input.GetKeyDown(KeyCode.S)) //(+1,0)
        {
            TryMovePosition(1);
        }
        if (Input.GetKeyDown(KeyCode.A)) //(0,-1)
        {
            TryMovePosition(2);
        }
        if(Input.GetKeyDown(KeyCode.W)) //(-1,0)
        {
            TryMovePosition(3);
        }
        
    }

}
