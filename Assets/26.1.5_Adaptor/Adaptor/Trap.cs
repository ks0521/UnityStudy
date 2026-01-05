using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adaptor
{
    public class Trap : MonoBehaviour, IBtninteractable
    {
        public virtual void Active()
        {
            Debug.Log("함정 작동");
        }
        private void OnTriggerEnter(Collider other)
        {
            Active();
        }
    }
}
