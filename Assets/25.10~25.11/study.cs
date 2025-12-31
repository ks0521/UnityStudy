using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class study : MonoBehaviour
{

    //FSM 유한 상태 머신
    //유한한 상태가 있고, 이것을 제어하는 방법.

    enum GameState
    {
        Break,
        Walk,
        Run
    }

    GameState currentState;
    GameState previousState; //직전 상태를 저장

    private void Start()
    {
        currentState = GameState.Walk;
        previousState = currentState;

        OnEnter();
    }
    static int maxEnergy = 30000;
    int nowEnergy = 12000;
    int Tick = 0;
    int distance = 0;
    void RunUpdate()
    {
        nowEnergy = nowEnergy - 3;
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentState = GameState.Walk;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            currentState = GameState.Break;
        }
        if (nowEnergy < 0)
        {
            Debug.Log("헥헥.... ");
            Debug.Log("더이상 뛸 수 없습니다! 걷겠습니다");
            currentState = GameState.Walk;
        }
    }
    void WalkUpdate()
    {
        nowEnergy = nowEnergy + 1;
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentState = GameState.Run;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            currentState = GameState.Break;
        }
        if(nowEnergy > maxEnergy)
        {
            Debug.Log("힘이 다 찼습니다! 다시 달립니다!");
            currentState = GameState.Run;
        }
    }
    void BreakUpdate()
    {
        nowEnergy = nowEnergy + 3;
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentState = GameState.Run;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            currentState = GameState.Break;
        }
        if(nowEnergy > maxEnergy-1500)
        {
            Debug.Log("힘이 거의 다 찼습니다! 다시 걷습니다!");
            currentState = GameState.Walk;
        }
    }
    void OnEnter()
    {
        if (currentState == GameState.Run)
        {
            Debug.Log("달리는 중입니다.");
            Debug.Log("W를 누르면 걸을 수 있습니다.");
        }
        else if(currentState == GameState.Walk)
        {
            Debug.Log("걷는중입니다.");
            Debug.Log("R을 누르면 뛸 수 있고, B를 누르면 쉴 수 있습니다.");
        }
        else if( currentState == GameState.Break)
        {
            Debug.Log("쉬는중입니다.");
            Debug.Log("W를 누르면 걸을 수 있습니다");
        }
    }
    private void Update()
    {
        distance += (int)currentState * 3;
        Tick++;
        if(Tick>300)
        {
            Debug.Log($"{currentState}중! 현재 에너지: {nowEnergy}\n총 이동거리: {distance}");
            Tick = 0;
        }
        //상태가 변경되었음.
        if (currentState != previousState)
        {
            OnEnter();
            previousState = currentState;
        }

        if (currentState == GameState.Run)
        {
            RunUpdate();
        }
        else if (currentState == GameState.Walk)
        {
            WalkUpdate();
        }
        else if (currentState == GameState.Break)
        {
            BreakUpdate();
        }
    }

}
