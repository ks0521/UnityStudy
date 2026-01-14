using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Adaptor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI_MVC
{
    /*
    MVC패턴 : 모델과 뷰, 컨트롤러의 역할을 분리하여 관리하는 패턴
    Model : PlayerData (데이터 관리)
    View : HPView (UI 관리)
    Controller : Player (데이터와 UI 연결)
    */

    public class Player_MVC : MonoBehaviour
    {
        public Image image;
        public TextMeshProUGUI hpText;
        public PlayerJobData jobData;
        public PlayerData data;
        public HPView_MVC hpView;
        private void Awake()
        {
            data.Set(jobData.defaultData);
            hpView.SetPlayer(this);
            data.OnHPChange += hpView.UpdateUI;
            hpView.UpdateUI();
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
        }
    }
}