using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Adaptor;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [CreateAssetMenu]
    public class PlayerJobData : ScriptableObject
    {
        public PlayerData defaultData;
        public Sprite sprite;
    }
    [System.Serializable]
    public class PlayerData
    {
        public float hp;
        public int atk;
        public event Action OnHPChange;
        public float HP
        {
            get { return hp; }
            set
            {
                hp = value;
                OnHPChange?.Invoke();
            }
        }
        public void Set(PlayerData data)
        {
            HP = data.hp;
            atk = data.atk;
        }
    }
    public class Player : MonoBehaviour
    {
        public Image image;
        public PlayerJobData jobData;
        public PlayerData data;
        private void Awake()
        {
            data.Set(jobData.defaultData);
            data.OnHPChange += UpdateUI;
        }
        void UpdateUI()
        {
            image.fillAmount = data.HP / jobData.defaultData.hp;
        }
        public void Hit()
        {
            data.HP -= 10;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Hit();
            }
        }
    }
}