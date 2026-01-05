using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using AbstractFactory;
using System;

namespace Adaptor
{
    
    public interface IBtninteractable
    {
        void Active();
    }
    public class Button : MonoBehaviour, IInteractable
    {
        public GameObject obj;
        public IBtninteractable interactable;

        private void Start()
        {
            interactable = obj.GetComponent<IBtninteractable>();
        }
        public void Interaction()
        {
            Debug.Log("딸깍");
            interactable.Active();
        }
    }
}

