using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MessageListener : MonoBehaviour
{
    public UnityEvent messageEvent;

    public Message message;

    public void OnMessageRaised()
    {
        messageEvent.Invoke();
    }

    private void OnEnable()
    {
        message.Register(this);
    }

    private void OnDisable()
    {
        message.Unregister(this);
    }
}
