using System;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    private Dictionary<string, Action> eventActions = new Dictionary<string, Action>();

    public void RegisterEvent(string eventName, Action callback)
    {
        if (eventActions.ContainsKey(eventName))
        {
            eventActions[eventName] += callback;
        }
        else
        {
            eventActions[eventName] = callback;
        }
    }

    public void UnregisterEvent(string eventName, Action callback)
    {
        if (eventActions.ContainsKey(eventName))
        {
            eventActions[eventName] -= callback;
            if (eventActions[eventName] == null)
            {
                eventActions.Remove(eventName);
            }
        }
    }

    public void TriggerEvent(string eventName)
    {
        if (eventActions.TryGetValue(eventName, out Action action))
        {
            action?.Invoke();
        }
    }
}
