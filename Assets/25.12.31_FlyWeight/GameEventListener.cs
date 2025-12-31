using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    //듣고싶은 이벤트
    public GameEvent gameEvent;

    public UnityEvent response;

    public void OnEvent()
    {
        response.Invoke();
    }
    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }
    private void OnDisable()
    {
        gameEvent.UnRegisterListener(this);
    }

}
