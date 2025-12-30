using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public abstract class State
{
    public StateMachineEx sm;
    public abstract void Start();
    public abstract void Stay();
    public abstract void End();
    public abstract void Init();

}

//public abstract class PlayerState : State
//{
//    public Player pl;
//    public override void Init()
//    {
//        pl = sm.GetOnwer<Player>();
//    }
//}
public abstract class MonsterState : State
{
    public MonsterEx mon;

    public override void Init()
    {
        mon = sm.GetOnwer<MonsterEx>();
    }
}

public class MonsterIdleState : MonsterState
{
    public override void Start()
    {
        Debug.Log("대기상태에 진입");
    }
    public override void Stay()
    {
        Debug.Log("대기 중");
        if (Input.GetKeyDown(KeyCode.V))
        {
            sm.ChagneState("Walk");
        }
    }
    public override void End()
    {
        Debug.Log("대기상태에 종료");
    }
}
public class MonsterWalkState : MonsterState
{
    IEnumerator walkCo;
    public MonsterWalkState()
    {
        walkCo = WalkCo();
    }
    IEnumerator WalkCo()
    {
        yield return new WaitForSeconds(2);
        sm.ChagneState("Idle");
    }
    public override void Start()
    {
        mon.StartCoroutine(walkCo);
    }

    public override void Stay()
    {
        mon.transform.Translate(Vector3.forward);
    }

    public override void End()
    {
        mon.StopCoroutine(walkCo);
    }
}

public class MonsterAttackState : MonsterState
{
    public override void Start()
    {
        Debug.Log("공격상태에 진입");
    }
    public override void Stay()
    {
        Debug.Log("공격중");
    }
    public override void End()
    {
        Debug.Log("공격상태에 종료");
    }
}

//StateMachine은 상태를 관리해주는 녀석입니다.
public class StateMachineEx
{
    object owner;
    public T GetOnwer<T>() where T : class, new()
    {
        return (T)owner;
    }
    State curState;
    Dictionary<string, State> stateDic;

    public StateMachineEx(object owner)
    {
        stateDic = new Dictionary<string, State>();
        this.owner = owner;
    }
    public void AddState(string name, State state)
    {
        state.sm = this;
        state.Init();
        stateDic.Add(name, state);
    }
    public void ChagneState(string name)
    {
        if (stateDic.ContainsKey(name) == false)
        {
            return;
        }
        if (curState != null)
        {
            curState.End();
        }
        curState = stateDic[name];
        curState.Start();
    }
    public void Stay()
    {
        curState?.Stay();
    }
}

public class MonsterEx : MonoBehaviour
{
    StateMachineEx sm;

    //상태
    //대기, 정찰, 공격, 죽음
    void Start()
    {
        sm = new StateMachineEx(this);
        sm.AddState("Idle", new MonsterIdleState());
        sm.AddState("Walk", new MonsterWalkState());
        sm.AddState("Attack", new MonsterAttackState());
        sm.ChagneState("Idle");
    }
    void Update()
    {
        sm.Stay();
    }
}
