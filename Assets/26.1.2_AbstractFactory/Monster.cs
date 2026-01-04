using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbstractFactory
{
    public abstract class AttackStrategy
    {
        public abstract void Attack();
    }

    public class RangedAttackStrategy : AttackStrategy
    {
        public GameObject bulletPrefeb;
        public RangedAttackStrategy()
        {
            //bulletPrefeb = prefeb;
        }
        public override void Attack()
        {
            //GameObject.Instantiate(bulletPrefeb);
            Debug.Log("원거리 공격");
        }
    }

    public class MeleeAttackStrategy : AttackStrategy
    {
        public override void Attack()
        {
            Debug.Log("근거리 공격");
        }
    }
    public abstract class MoveStrategy
    {
        public abstract void Move();
    }
    public class RotateStrategy : MoveStrategy 
    {
        public override void Move()
        {
            //객체 이동...
            Debug.Log("회전");
        }
    }
    public class WalkStrategy : MoveStrategy
    {
        public override void Move()
        {
            Debug.Log("걷기");
        }
    }
    public abstract class MonsterStrategyFactory
    {
        public abstract AttackStrategy CreateAttackStrategy();
        public abstract MoveStrategy CreateMoveStrategy();
    }
    //추상 팩토리 - 해당 팩토리를 사용하는 객체는 회전 / 원거리 공격만 가능(구현의 제약을 걺)
    public class RotateRangeMonsterFactory : MonsterStrategyFactory
    {
        public override AttackStrategy CreateAttackStrategy()
        {
            return new RangedAttackStrategy();
        }
        public override MoveStrategy CreateMoveStrategy()
        {
            return new RotateStrategy();
        }
    }
    public class WalkMeleeMonsterFactory : MonsterStrategyFactory
    {
        public override AttackStrategy CreateAttackStrategy()
        {
            return new MeleeAttackStrategy();
        }
        public override MoveStrategy CreateMoveStrategy()
        {
            return new WalkStrategy();
        }
    }

    //프로토타입 패턴 이용해서 해당부분 스택화 시키기
    public class StrategyProvider
    {
        public static MonsterStrategyFactory CreateStrategyFactory(int type)
        {
            MonsterStrategyFactory factory = null;
            switch (type)
            {
                case 0:
                    factory = new RotateRangeMonsterFactory();
                    break;
                case 1:
                    factory = new WalkMeleeMonsterFactory();
                    break;
            }
            return factory;
        }
    }
    public class Monster : MonoBehaviour
    {
        public int type = 0;
        public AttackStrategy attackStrategy;
        public MoveStrategy moveStrategy;
        void Attack() { attackStrategy?.Attack(); }
        void Move() {  moveStrategy?.Move(); }
        private void Start()
        {
            MonsterStrategyFactory msf = StrategyProvider.CreateStrategyFactory(type);
            attackStrategy = msf.CreateAttackStrategy();
            moveStrategy = msf.CreateMoveStrategy();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) Attack();
            if (Input.GetKeyDown(KeyCode.M)) Move();
        }
    }

}
