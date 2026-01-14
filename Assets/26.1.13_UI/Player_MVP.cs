using System;
using System.Collections.Generic;
using Adaptor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace UI_MVP
{
    /*
    MVP패턴 : 모델과 뷰, 프레젠터의 역할을 분리하여 관리하는 패턴
    Model : PlayerData (데이터 관리)
    View : HPView_MVC (UI 관리)
    Presenter : Player_MVP (데이터와 UI 연결 - 모델과 뷰를 중계)
    MVC패턴과의 차이점: 뷰가 모델의 존재를 모름(뷰에서 모델의 의존도 제거)
    */

    public class Player_MVP : MonoBehaviour
    {
        public Image image;
        public TextMeshProUGUI hpText;
        public List<PlayerJobData> jobDataList;
        public PlayerJobData jobData;
        public PlayerData data;
        public HPView_MVP hpView;
        public event Action<Sprite> OnCharacterChange;
        private void Awake()
        {
            SetJob(0);
            data.Set(jobData.defaultData);
        }
        public void SetJob(int index)
        {
            if (index < 0 || index >= jobDataList.Count)
                return;
            jobData = jobDataList[index];
            OnCharacterChange?.Invoke(jobData.sprite);
        }
        public void Hit()
        {
            data.Hp -= 10;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Hit();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetJob(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetJob(1);
            }
        }
    }
}