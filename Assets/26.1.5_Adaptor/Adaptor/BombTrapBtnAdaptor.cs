using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adaptor
{
    public class BombTrapBtnAdaptor : MonoBehaviour, IBtninteractable
    {
        BombTrap bombTrap;
        public void Awake()
        {
            bombTrap = GetComponent<BombTrap>();
        }
        public void Active()
        {
            bombTrap.Active();
        }

    }

}
