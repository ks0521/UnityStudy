using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace UI_MVC
{
    public class HPView_MVC : MonoBehaviour
    {
        public Image Hpbar;
        public TextMeshProUGUI Hptext;
        public Player_MVC player;

        public void SetPlayer(Player_MVC target)
        {
            player = target;
        }
        public void UpdateUI()
        {
            Hpbar.fillAmount = player.data.Hp / player.jobData.defaultData.hp;
            Hptext.text = player.data.Hp.ToString();
        }
    }
}
