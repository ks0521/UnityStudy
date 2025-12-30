using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public abstract class MonsterState
    {
        public MonsterStateMachine sm;

        public abstract void Start();
        public abstract void Stay();
        public abstract void End();
    }
    public class MonsterIdleState : MonsterState
    {
        public override void Start()
        {
            Debug.Log("대기상태 진입");
        }
        public override void Stay()
        {
            Debug.Log("대기 중");
            if (Input.GetKeyDown(KeyCode.W))
            {
                sm.ChangeState("Walk");
            }
        }
        public override void End()
        {
            Debug.Log("대기상태 벗어남");
        }
    }
    public class MonsterWalkState : MonsterState
    {
        IEnumerator walkCo;
        IEnumerator WalkCo()
        {
            yield return new WaitForSeconds(2);
            sm.ChangeState("Idle");
        }
        public override void Start()
        {
            walkCo = WalkCo();
            sm.owner.StartCoroutine(WalkCo());
            Debug.Log("걷기상태 진입");
        }
        public override void Stay()
        {
            Debug.Log("걷는 중");
            sm.owner.transform.Translate(Vector3.forward * Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.I))
            {
                sm.ChangeState("Idle");
            }
        }
        public override void End()
        {
            sm.owner.StopCoroutine(walkCo);
            Debug.Log("걷는상태 벗어남");
        }
    }
    public class MonsterAttackState : MonsterState
    {
        public override void Start()
        {
            Debug.Log("공격상태 진입");
        }
        public override void Stay()
        {
            Debug.Log("공격 중");
            if (Input.GetKeyDown(KeyCode.I))
            {
                sm.ChangeState("Idle");
            }
        }
        public override void End()
        {
            Debug.Log("공격상태 벗어남");
        }
    }

    //StateMachine은 닉값하게 상태를 관리
    public class MonsterStateMachine
    {
        public MonsterState curState;
        public MonsterBase owner;
        Dictionary<string, MonsterState> stateDic;
        public MonsterStateMachine(MonsterBase mon)
        {
            stateDic = new Dictionary<string, MonsterState>();
            owner = mon;
            
        }
        public void AddState(string name, MonsterState state)
        {
            stateDic.Add(name, state);
        }
        public void ChangeState(string name)
        {
            if(curState != null) curState.End();
            if (stateDic.ContainsKey(name) == false) return;
            curState = stateDic[name];
            curState.sm = this;
            curState.Start();
        }
        public void Stay()
        {
            curState?.Stay();
        }
    }
    public class MonsterBase : MonoBehaviour
    {
        //몬스터 상태 : 대기, 정찰, 공격, 죽음
        MonsterStateMachine stateMachine;
        void Start()
        {
            stateMachine = new MonsterStateMachine(this);
            stateMachine.AddState("Idle",new MonsterIdleState());
            stateMachine.AddState("Walk",new MonsterWalkState());
            stateMachine.AddState("Attack",new MonsterAttackState());
            stateMachine.ChangeState("Idle");
        }

        void Update()
        {
            stateMachine.Stay();
        }
    }

}
