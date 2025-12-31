using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day25_11_5 : MonoBehaviour
{
    interface IFlyable { void Fly(); }
    interface ISwimable { void Swim(); }
    interface IWalkable { void Walk(); }
    class Sea
    {
        public void Swim(ISwimable target)
        {
            target.Swim();
        }
    }
    class Sky
    {
        public void Fly(IFlyable target)
        {
            target.Fly();
        }
    }
    class Land
    {
        public void Walk(IWalkable target)
        {
            target.Walk();
        }
    }
    class Monster
    {
        string name;
        int atk;
        int hp;
    }
    class Dragon : Monster, IFlyable, ISwimable, IWalkable
    {
        public void Fly()
        {
            Debug.Log("드래곤 펄럭펄럭");
        }
        public void Swim()
        {
            Debug.Log("드래곤 첨벙첨벙");
        }
        public void Walk()
        {
            Debug.Log("드래곤 뚜벅뚜벅");
        }
    }
    class Murlok : Monster, ISwimable, IWalkable
    {
        public void Swim()
        {
            Debug.Log("멀록 첨벙첨벙");
        }
        public void Walk()
        {
            Debug.Log("멀록 뚜벅뚜벅");
        }
    }
    class Piranha : Monster, ISwimable
    {
        public void Swim()
        {
            Debug.Log("피라냐 첨벙첨벙");
        }
    }
    private void Start()
    {
        Land land = new Land();
        Sky sky = new Sky();
        Sea sea = new Sea();
        Dragon dragon = new Dragon();
        Murlok murlok  = new Murlok();
        Piranha piranha = new Piranha();
        land.Walk(dragon);
        land.Walk(murlok);
        sky.Fly(dragon);
        sea.Swim(dragon);
        sea.Swim(murlok);   
        sea.Swim(piranha);
    }
}
