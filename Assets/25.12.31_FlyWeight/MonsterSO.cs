using ObjectPooling;
using StateMachine;
using StateMachine2;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SO
{

    //과제2: 몬스터 있고, 몬스터 속성(멤버변수)존재
    //공유되어야 하는 데이터(MaxHp)와
    //공유되면 안되는 객체만의 데이터(hp)
    //2개를 분리해서 SO로 빼기
    #region 상태머신 구현부
    public abstract class State
    {
        public StateMachine sm;
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
        public abstract void Init();
    }
    public abstract class MonsterState : State
    {
        public MonsterSO mon;
        public override void Init()
        {
            mon = sm.GetOwner<MonsterSO>();
        }
    }
    public class StateMachine
    {
        public State curState;
        public object owner;
        public Dictionary<string, State> stateDic;
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
            if (curState != null) curState.Exit();
            if (stateDic.ContainsKey(name) == false) return;
            curState = stateDic[name];
            curState.Enter();
        }
        public void Update()
        {
            curState.Update();
        }
    }
    #endregion

    #region 몬스터 공격상태
    public class MonsterAttackState : MonsterState
    {
        public AttackStrategy attackStrategy;
        IEnumerator attackCo;
        public MonsterAttackState(AttackStrategy stratagy)
        {
            attackStrategy = stratagy;
            attackCo = Attack();
        }
        IEnumerator Attack()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                Debug.Log("총알 발사");
                attackStrategy.Attack(mon.firePoint.transform);
            }
        }
        public override void Enter()
        {
            Debug.Log("몬스터 공격 시작");
            mon.StartCoroutine(attackCo);
        }

        public override void Exit()
        {
            mon.StopCoroutine(attackCo);
            Debug.Log("몬스터 사망, 공격중지");
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.I))
            {
                sm.ChangeState("Idle");
            }
            // 숫자 1, 2, 3을 누를때마다 총알 종류 전환
            attackStrategy.Update();
        }
    }
    public abstract class AttackStrategy
    {
        public abstract void Attack(Transform firePoint);
        public abstract void Update();
    }
    public class RangeAttackStrategy : AttackStrategy
    {
        public Dictionary<string, ObjectPool> bulletTypes;
        public ObjectPool bullets;
        public RangeAttackStrategy()
        {
            bulletTypes = new Dictionary<string, ObjectPool>();
            bulletTypes.Add("Bullet", PoolManager.poolDic["Bullet"]);
            bulletTypes.Add("BlueBullet", PoolManager.poolDic["BlueBullet"]);
            bulletTypes.Add("RedBullet", PoolManager.poolDic["RedBullet"]);
            bullets = bulletTypes["Bullet"];
        }
        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (bulletTypes.ContainsKey("Bullet") == false) return;
                bullets = bulletTypes["Bullet"];
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (bulletTypes.ContainsKey("BlueBullet") == false) return;
                bullets = bulletTypes["BlueBullet"];
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (bulletTypes.ContainsKey("RedBullet") == false) return;
                bullets = bulletTypes["RedBullet"];
            }
        }
        public override void Attack(Transform firePoint) 
        {
            bullets.UsePool(firePoint.position, firePoint.rotation);
        }
    }


    #endregion
    #region 몬스터 이동상태

    public class MonsterMoveState : MonsterState
    {
        public override void Enter()
        {
            Debug.Log("몬스터 이동 시작");
        }

        public override void Exit() { }

        public override void Update()
        {
            mon.transform.Translate(Vector3.forward * Time.deltaTime * 2);
            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.M))
            {
                sm.ChangeState("Idle");
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                sm.ChangeState("Die");
            }
        }
    }
    public class MonsterIdleState : MonsterState
    {
        public override void Enter()
        {
            Debug.Log("대기상태 진입");
        }
        public override void Exit() { }
        public override void Update()
        {
            Debug.Log("대기중");
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (sm.stateDic.ContainsKey("Move") == false) return;
                sm.ChangeState("Move");
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (sm.stateDic.ContainsKey("Attack") == false) return;
                sm.ChangeState("Attack");
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                sm.ChangeState("Die");
            }
        }
    }
    public class MonsterDieState : MonsterState
    {
        public override void Enter()
        {
            mon.Die();
        }

        public override void Exit() { }

        public override void Update() { }
    }
    #endregion
    public class MonsterSO : MonoBehaviour
    {
        public int hp = 100;
        public MonsterData data;
        public StateMachine behaviorSm;
        public StateMachine attackSm;
        public Transform firePoint;
        public void Die()
        {
            DropItem();
            Destroy(gameObject);
        }

        public virtual void Init()
        {
            attackSm.AddState("Attack", new MonsterAttackState(new RangeAttackStrategy()));

        }
        public void DropItem()
        {
            int randIndex = UnityEngine.Random.Range(0, data.dropList.Count);
            Debug.Log(data.dropList[randIndex]);
        }
        void Start()
        {
            hp = 100;
            behaviorSm = new StateMachine(this);
            behaviorSm.AddState("Idle", new MonsterIdleState());
            behaviorSm.AddState("Move", new MonsterMoveState());
            behaviorSm.AddState("Die", new MonsterDieState());
            attackSm = new StateMachine(this);
            attackSm.AddState("Idle", new MonsterIdleState());

            behaviorSm.ChangeState("Idle");
            attackSm.ChangeState("Idle");
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            if (hp <= 0) behaviorSm.ChangeState("Die");
            behaviorSm?.Update();
            attackSm?.Update();
        }
    }
    public class Dragon : MonsterSO
    {
        public override void Init()
        {
            attackSm.AddState("Attack", new MonsterAttackState(new RangeAttackStrategy()));
        }
    }
}
