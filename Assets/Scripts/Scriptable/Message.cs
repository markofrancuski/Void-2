using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Message" , menuName = "", order = 0)]
public class Message : ScriptableObject
{
    public List<MessageListener> listeners = new List<MessageListener>();

    public void Raise()
    {
        for (int i = listeners.Count; i >= 0; i--)
        {
            listeners[i].OnMessageRaised();
        }
    }

    public void Register(MessageListener listener)
    {
        listeners.Add(listener);
    }
    public void Unregister(MessageListener listener)
    {
        listeners.Remove(listener);
    }
}
