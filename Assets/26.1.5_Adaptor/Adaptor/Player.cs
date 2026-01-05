using System.Collections;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

namespace Adaptor
{
    public class Weapon
    {

    }
    public class Sword : Weapon
    {

    }
    public class Claymore : Sword
    {

    }
    public class Staff : Weapon
    {

    }
    public class Arrow : Weapon
    {

    }
    public class Armor
    {

    }
    public class PaddedArmor : Armor
    {

    }
    public class HeavyArmor : Armor
    {

    }
    public class LinenArmor : Armor
    {

    }
    public enum PlayerJob
    {
        Warrior,
        Berserker,
        Archer,
        Mage
    }
    public abstract class ItemFactory
    {
        public abstract Weapon GetWeapons();
        public abstract Armor GetArmors();
    }
    public class WarriorFactory : ItemFactory
    {
        //sealed : 자식 클래스에서 변경(override) 불가
        public sealed override Weapon GetWeapons()
        {
            return CustomCreatWeapon();
        }
        public sealed override Armor GetArmors()
        {
            return CustomCreatArmor();
        }
        //자식 클래스가 있다면 자식에서 무기와 방어구를 변경할 수 있도록 수정
        public virtual HeavyArmor CustomCreatArmor()
        {
            return new HeavyArmor();
        }
        public virtual Sword CustomCreatWeapon()
        {
            return new Sword();
        }
    }
    public class BerserkerFactory : WarriorFactory
    {
        public override Sword CustomCreatWeapon()
        {
            return new Claymore();
        }
    }
    public class ArchorFactory : ItemFactory
    {
        public override Weapon GetWeapons()
        {
            return new Arrow();
        }
        public override Armor GetArmors()
        {
            return new PaddedArmor();
        }
    }
    public class MageFactory : ItemFactory
    {
        public override Armor GetArmors()
        {
            return new LinenArmor();
        }

        public override Weapon GetWeapons()
        {
            return new Staff();
        }
    }
    public static class ItemFactoryProvider
    {
        public static ItemFactory FindJob(PlayerJob job)
        {
            switch (job)
            {
                case PlayerJob.Warrior:
                    return new WarriorFactory();
                case PlayerJob.Berserker:
                    return new BerserkerFactory();
                case PlayerJob.Archer:
                    return new ArchorFactory();
                case PlayerJob.Mage:
                    return new MageFactory();
                default:
                    return null;
            }
        }
    }
    public interface IInteractable
    {
        public void Interaction();
    }
    public class Player : MonoBehaviour
    {
        public PlayerJob job;

        Weapon weapon;
        Armor armor;
        ItemFactory factory;
        private void Start()
        {
            factory = ItemFactoryProvider.FindJob(job);
            if (factory == null)
            {
                Debug.LogWarning("무직!!");
                return;
            }
            weapon = factory.GetWeapons();
            armor = factory.GetArmors();
            Debug.Log(weapon.GetType());
            Debug.Log(armor.GetType());
        }
        public void OnTriggerEnter(Collider other)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interaction();
            }
        }
    }

}
