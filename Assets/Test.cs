using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.UIElements;


public interface ISwitchInteractable
{
    void Execute();
}
public class Switch
{
    ISwitchInteractable interactable;

    public Switch(ISwitchInteractable interactable)
    {
        this.interactable = interactable;
    }
    public void Click()
    {
        interactable.Execute();
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
        Debug.Log("²¨Áü");
    }
}
public class BulbOffState : BulbState
{
    public override void Execute()
    {
        Debug.Log("ÄÑÁü");
    }
}
public class BulbFlickState : BulbState
{
    public override void Execute()
    {
        Debug.Log("±ôºý±ôºý °Å¸®°Ô ÇÔ");
    }
}


public class Bulb : ISwitchInteractable//Àü±¸
{
    public BulbState curState;
    public List<BulbState> stateList;
    public int curIndex;
    public Bulb()
    {
        curIndex = 0;
        stateList = new List<BulbState>();
        CustomSetthing();
        curState = stateList[0];
    }
    public virtual void CustomSetthing()
    {
        stateList.Add(new BulbOffState());
        stateList.Add(new BulbOnState());
    }
    public void NextState()
    {
        curIndex++;
        if(curIndex >= stateList.Count)
        {
            curIndex = 0;
        }
        curState = stateList[curIndex];
    }
    public void Execute()
    {
        curState?.Execute();
        NextState();
    }
}

public class ChristmasBulb : Bulb
{
    public override void CustomSetthing()
    {
        stateList.Add(new BulbOffState());
        stateList.Add(new BulbFlickState());
        stateList.Add(new BulbOnState());
    }
}

public class Test : MonoBehaviour
{
    Switch cSwitch;
    private void Start()
    {
        Bulb bulb = new Bulb();
        ChristmasBulb christmasBulb = new ChristmasBulb();
        cSwitch = new Switch(christmasBulb);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            cSwitch.Click();
        }
    }
}
