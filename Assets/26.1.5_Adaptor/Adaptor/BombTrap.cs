using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adaptor
{
    public class BombTrap : Trap
    {
        public override void Active()
        {
            Debug.Log("펑!");
        }
    }

}
