using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static project;

public class Day25_11_11 : MonoBehaviour
{
    class MonsterSpwner<T> where T : Monster, new()
    {
        public T Spawn()
        {
            T result = new T();
            result.Initial();
            return result;
        }
    }
    class MonsterSpawner2<T> where T : Monster, new()
    {
        public static T Spawn()
        {
            T monster = new T();
            monster.Initial();
            return monster;
        }
    }

    MonsterSpwner<Dragon> dragonSpawner;
    class Monster
    {
        public void Initial() { }
    }
    class Dragon : Monster
    {

    }
    class Skeleton : Monster { }
    // Start is called before the first frame update
    void Start()
    {
        dragonSpawner = new MonsterSpwner<Dragon>();
        dragonSpawner.Spawn();
        Monster skeleton = MonsterSpawner2<Skeleton>.Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
