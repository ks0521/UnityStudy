using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adaptor
{
    public class CustomDoorBtnAdapter : MonoBehaviour, IBtninteractable
    {
        public CustomDoor door;

        public void Awake()
        {
            door = GetComponent<CustomDoor>();
        }
        public void Active()
        {
            door.Interaction();
        }

    }

}
