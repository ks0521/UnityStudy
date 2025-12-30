using StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using ObjectPooling;
using System.Runtime.CompilerServices;

namespace StateMachine2
{
    public abstract class State
    {
        public StateMachine sm;
        public abstract void Start();
        public abstract void Stay();
        public abstract void End();
        public abstract void Init();
    }
    public abstract class MonsterState : State
    {
        public Monster mon;
        public override void Init()
        {
            mon = sm.GetOwner<Monster>();
        }
    }
    public class StateMachine
    {
        public State curState;
        object owner;
        Dictionary<string, State> stateDic;
        public T GetOwner<T>() where T : class, new()
        {
            return (T)owner;
        }
        public StateMachine(object owner)
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
        public void ChangeState(string name)
        {
            if (curState != null) curState.End();
            if (stateDic.ContainsKey(name) == false) return;
            curState = stateDic[name];
            curState.Start();
        }
        public void Stay()
        {
            curState?.Stay();
        }
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
            yield return new WaitForSeconds(7);
            Debug.Log("코루틴 끝");
            sm.ChangeState("Idle");
        }
        public MonsterWalkState()
        {
            walkCo = WalkCo();
        }
        public override void Start()
        {
            mon.StartCoroutine(walkCo);
            Debug.Log("걷기상태 진입");
        }
        public override void Stay()
        {
            Debug.Log("걷는 중");
            mon.transform.Translate(Vector3.forward * Time.deltaTime * 2);
            if (Input.GetKeyDown(KeyCode.A))
            {
                sm.ChangeState("Attack");
            }
        }
        public override void End()
        {
            mon.StopCoroutine(walkCo);
            Debug.Log("걷는상태 벗어남");
        }
    }
    public class MonsterDieState : MonsterState
    {
        IEnumerator dieCo()
        {
            yield return new WaitForSeconds(3);
            mon.transform.position = Vector3.zero;
            sm.ChangeState("Idle");
        }
        public override void Start()
        {
            Debug.Log("몬스터 사망");
            mon.StartCoroutine(dieCo());
        }

        public override void Stay()
        {
            Debug.Log("몬스터 부활중....");
        }
        public override void End()
        {
            Debug.Log("몬스터 부활 완료!");
        }
    }
    public class MonsterAttackState : MonsterState
    {
        public AttackStrategy attackStrategy;
        public Dictionary<string, AttackStrategy> attackDic;
        public MonsterAttackState()
        {
            attackDic = new Dictionary<string, AttackStrategy>();
            attackDic.Add("Bullet",new AttackOneStrategy());
            attackDic.Add("BlueBullet", new AttackTwoStrategy());
            attackDic.Add("RedBullet", new AttackThreeStrategy());
            attackStrategy = attackDic["Bullet"];
        }
        public override void Start()
        {
            Debug.Log("공격상태 진입");
        }
        public override void Stay()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                attackStrategy.Attack(mon.transform);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                sm.ChangeState("Die");
            }
            // 숫자 1, 2, 3을 누를때마다 총알 종류 전환
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (attackDic.ContainsKey("Bullet") == false) return;
                attackStrategy = attackDic["Bullet"];
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (attackDic.ContainsKey("BlueBullet") == false) return;
                attackStrategy = attackDic["BlueBullet"];
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (attackDic.ContainsKey("RedBullet") == false) return;
                attackStrategy = attackDic["RedBullet"];
            }
        }

        public override void End()
        {
            Debug.Log("공격상태 벗어남");
        }
    }
    public abstract class AttackStrategy
    {
        public abstract void Attack(Transform firePoint);
    }
    public class AttackOneStrategy : AttackStrategy
    {
        public override void Attack(Transform firePoint)
        {
            PoolManager.poolDic["Bullet"].UsePool(firePoint.position, firePoint.transform.rotation);
        }
    }
    public class AttackTwoStrategy : AttackStrategy
    {
        public override void Attack(Transform firePoint)
        {
            PoolManager.poolDic["BlueBullet"].UsePool(firePoint.position, firePoint.transform.rotation);
        }
    }
    public class AttackThreeStrategy : AttackStrategy
    {
        public override void Attack(Transform firePoint)
        {
            PoolManager.poolDic["RedBullet"].UsePool(firePoint.position, firePoint.transform.rotation);
        }
    }
    
    public class Monster : MonoBehaviour
    {
        //몬스터 상태 : 대기, 정찰, 공격, 죽음
        StateMachine stateMachine;
        
        void Start()
        {
            stateMachine = new StateMachine(this);
            stateMachine.AddState("Idle", new MonsterIdleState());
            stateMachine.AddState("Walk", new MonsterWalkState());
            stateMachine.AddState("Attack", new MonsterAttackState());
            stateMachine.AddState("Die", new MonsterDieState());
            stateMachine.ChangeState("Idle");
        }

        void Update()
        {
            stateMachine.Stay();
        }
    }

}
