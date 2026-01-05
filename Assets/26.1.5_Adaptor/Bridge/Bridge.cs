using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//브리지 패턴 : 구현부에서 추상층을 분리해 각각 독립적으로 변형할 수 있게 해주는 패턴

namespace Bridge
{
    public class Monster
    {
        Job job;
        Species species;
    }
    public class Job
    {

    }
    public class Warrior : Job
    {
        
    }
    public class Archer : Job
    {
        
    }
    public class Species
    {

    }
    public class Orc : Species
    {

    }
    public class Elf : Species
    {

    }
    public class Goblin : Species
    {

    }
    public class Bridge : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
