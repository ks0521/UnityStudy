using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day25_11_4 : MonoBehaviour
{
    class Animal
    {
        string name;
        public virtual void Eat()
        {
            Debug.Log("먹는다");
        }
    }

    class Carnivore : Animal
    {
        public override void Eat()
        {
            Debug.Log("고기먹는다");
        }
    }
    class Herbivore : Animal
    {
        public override void Eat()
        {
            Debug.Log("풀먹는다");
        }
    }
    void Start()
    {
        Animal[] animals = new Animal[2];
        animals[0] = new Carnivore();
        animals[1] = new Herbivore();
        Carnivore c = new Carnivore();
        animals[0].Eat();
        animals[1].Eat();
        Debug.Log(c is  Animal);
    }
}
