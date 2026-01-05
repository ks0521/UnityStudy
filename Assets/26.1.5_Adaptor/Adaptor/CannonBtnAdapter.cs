using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adaptor
{
    public class CannonBtnAdapter : MonoBehaviour, IBtninteractable
    {
        Cannon cannon;

        public void Active()
        {
            cannon.Fire();
        }

        void Start()
        {
            cannon = GetComponent<Cannon>();
        }

    }

}
