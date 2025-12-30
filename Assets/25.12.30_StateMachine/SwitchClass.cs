using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public interface ISwitchinteractableInterface
    {
        void Execute();
    }
    public class Switch //스위치
    {
        Bulb bulb;
        ISwitchinteractableInterface interactable;
        public Switch(ISwitchinteractableInterface interactable)
        {
            this.interactable = interactable;
        }
        public void Click()
        {
            interactable?.Execute();
        }
    }
    public class Bulb : ISwitchinteractableInterface //전구
    {
        //on / off 상태를 가질 수 있음
        public BulbState curState;
        public List<BulbState> states;
        public int curIndex;
        public Bulb()
        {
            curIndex = 0;
            states = new List<BulbState>();
            CustomSetting();
            curState = states[0];
        }
        public virtual void CustomSetting()
        {
            states.Add(new BulbOffState());
            states.Add(new BulbOnState());

        }
        public void NextState()
        {
            curIndex++;
            if (curIndex >= states.Count)
            {
                curIndex = 0;
            }
            curState = states[curIndex];
        }
        public void Execute()
        {

            curState?.Execute();
            NextState();
        }
    }
    public class ChristmasBulb : Bulb
    {
        public override void CustomSetting()
        {
            states.Add(new BulbOffState());
            states.Add(new BulbOnState());
            states.Add(new BulbFlickState());
        }
    }
    public abstract class BulbState
    {
        public abstract void Execute();
    }

    public class BulbOnState : BulbState
    {
        public override void Execute()
        {
            Debug.Log("반짝임");
        }
    }
    public class BulbOffState : BulbState
    {
        public override void Execute()
        {
            Debug.Log("깜깜");
        }
    }
    public class BulbFlickState: BulbState
    {
        public override void Execute()
        {
            Debug.Log("깜빡임");
        }
    }
    public class SwitchClass : MonoBehaviour
    {
        Switch curSwitch;
        //상태 패턴 : 상태를 일반화해서 처리하는 패턴
        void Start()
        {
            curSwitch = new Switch(new ChristmasBulb());
            //curSwitch.Click();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                curSwitch.Click();
            }
        }
    }

}
