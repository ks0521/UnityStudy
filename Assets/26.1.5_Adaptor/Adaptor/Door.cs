using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adaptor
{
    public class Door : MonoBehaviour, IInteractable
    {
        public bool isOpen;

        public void Open()
        {
            Debug.Log("열렸다");
        }
        public void Close()
        {
            Debug.Log("닫혔다");
        }

        public void Interaction()
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
            isOpen = !isOpen;
        }
    }

}
