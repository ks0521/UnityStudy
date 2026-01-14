using UnityEngine;

namespace UI_MVP
{
    public class PlayerUIPresenter : MonoBehaviour
    {
        /*
        프레젠터는 뷰와 모델의 정보를 알고 있어야 함
        즉 , 참조하고 있어야 함
        */
        public HPView_MVP hPView;
        public Player_MVP player;
        private void Start()
        {
            player.data.OnHPChange += RefreshUI;
            RefreshUI();
        }
        public void RefreshUI()
        {
            hPView.UpdateUI(player.data.Hp, player.jobData.defaultData.hp);
        }
    }
}
