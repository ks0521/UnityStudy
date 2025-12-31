using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//상태패턴, 전략패턴
namespace sm
{

    #region 공격전략
    public abstract class AttackStrategy
    {
        public abstract void Attack(Player player);
    }
    public class MeleeAttackStrategy : AttackStrategy
    {

        public override void Attack(Player player)
        {
            Debug.Log("근거리 공격");
        }
    }

    public class ClawAttackStrategy : MeleeAttackStrategy
    {
        public GameObject skillPrafab;
        public Player pl;

        IEnumerator SkillChangeCo()
        {
            yield return new WaitForSeconds(3);
            pl.attackStrategy = new MeleeAttackStrategy();
        }
        public override void Attack(Player player)
        {
            pl = player;
            GameObject.Instantiate(skillPrafab, player.transform.position, player.transform.rotation);
            player.transform.Translate(Vector3.back * 2);
            Debug.Log("클로 공격!");
            player.StartCoroutine(SkillChangeCo());
        }
    }
    public class RangeAttackStrategy : AttackStrategy
    {
        public override void Attack(Player player)
        {
            Debug.Log("원거리 공격");
        }
    }
    #endregion

    public class PlayerState
    {
        public StateMachine sm;
        public virtual void Start()
        {

        }
        public virtual void Stay()
        {

        }
        public virtual void End()
        {

        }
    }

    public class PlayerIdleState : PlayerState
    {
        public override void Start()
        {
            Debug.Log("대기상태 진입");
        }
        public override void Stay()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                sm.ChangeState(Player.StateType.Attack);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                sm.ChangeState(Player.StateType.Move);
            }
        }
    }

    public class PlayerMoveState : PlayerState
    {
        public float speed = 5;
        public override void Start()
        {
            Debug.Log("이동상태 진입");
        }
        public override void Stay()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                sm.ChangeState(Player.StateType.Idle);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                sm.ChangeState(Player.StateType.Idle);
            }
            if (Input.GetKey(KeyCode.W))
            {
                sm.owner.transform.Translate(Vector3.forward * Time.deltaTime * speed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                sm.owner.transform.Translate(Vector3.back * Time.deltaTime * speed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                sm.owner.transform.Translate(Vector3.left * Time.deltaTime * speed);
            }
            if (Input.GetKey(KeyCode.D))
            {
                sm.owner.transform.Translate(Vector3.right * Time.deltaTime * speed);
            }
        }
    }
    public class PlayerAttackState : PlayerState
    {
        public void Attack()
        {
            Player player = sm.owner;
            player.attackStrategy?.Attack(player);
        }
        public override void Start()
        {
            Debug.Log("공격상태 진입");
            Attack();
            sm.ChangeState(Player.StateType.Idle);
        }
    }


    public class StateMachine
    {
        public Player owner;
        public PlayerState curState;
        public Dictionary<Player.StateType, PlayerState> stateDic;

        public StateMachine(Player owner)
        {
            this.owner = owner;
            stateDic = new Dictionary<Player.StateType, PlayerState>();
        }
        public void AddState(Player.StateType type, PlayerState state)
        {
            state.sm = this;
            stateDic.Add(type, state);
        }
        public void ChangeState(Player.StateType type)
        {
            if (curState != null)
            {
                curState.End();
            }
            curState = stateDic[type];
            curState.Start();
        }
        public void Stay()
        {
            curState?.Stay();
        }
    }

    //스킬(공격전략)을 하나 파생시켜서 적용해보기 합시다.
    public class Player : MonoBehaviour
    {
        public enum StateType
        {
            Idle,
            Attack,
            Move
        }
        StateMachine sm;
        public AttackStrategy attackStrategy = null;

        private void Start()
        {
            attackStrategy = new MeleeAttackStrategy();

            sm = new StateMachine(this);
            sm.AddState(StateType.Idle, new PlayerIdleState());
            sm.AddState(StateType.Attack, new PlayerAttackState());
            sm.AddState(StateType.Move, new PlayerMoveState());
            sm.ChangeState(StateType.Idle);
        }

        void Update()
        {
            sm.Stay();
        }


        private void OnTriggerEnter(Collider other)
        {
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                attackStrategy = item.attackStrategy;
                Destroy(other.gameObject);
            }
        }
    }
}
