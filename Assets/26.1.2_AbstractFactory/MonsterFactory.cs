using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbstractFactory
{
    public class MonsterFactory : MonoBehaviour
    {
        public Monster mon;

        public void Spawn(int type)
        {
            Monster newMon = Instantiate(mon, transform.position, transform.rotation);
            newMon.type = type;
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Spawn(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Spawn(1);
            }
        }
    }

}
