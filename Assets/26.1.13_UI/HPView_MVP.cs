using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace UI_MVP
{
    public class HPView_MVP : MonoBehaviour
    {
        public Image Hpbar;
        public TextMeshProUGUI Hptext;


        public void UpdateUI(float currentHP, float maxHP)
        {
            Hpbar.fillAmount = currentHP / maxHP;
            Hptext.text = $"{currentHP} / {maxHP}";
        }
    }
}
