using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Event",menuName = "SO/GameEvent")]
public class GameEvent : ScriptableObject
{
    private List<GameEventListener> listeners = new List<GameEventListener>();

    public void Event()
    {
        foreach(GameEventListener listener in listeners)
        {
            listener.OnEvent();
        }
    }
    public void RegisterListener(GameEventListener eventListener)
    {
        listeners.Add(eventListener);
    }
    public void UnRegisterListener(GameEventListener eventListener)
    {
        listeners.Remove(eventListener);
    }
}
