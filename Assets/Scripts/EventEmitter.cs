using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventEmitter
{
    private static Dictionary<string, Delegate> events = new Dictionary<string, Delegate>();

    // Subscribe (No parameters)
    public static void On(string eventName, Action listener)
    {
        if (!events.ContainsKey(eventName))
            events[eventName] = listener;
        else
            events[eventName] = (Action)events[eventName] + listener;
    }

    // Subscribe (With generic parameter)
    public static void On<T>(string eventName, Action<T> listener)
    {
        if (!events.ContainsKey(eventName))
            events[eventName] = listener;
        else
            events[eventName] = (Action<T>)events[eventName] + listener;
    }

    // Unsubscribe (No parameters)
    public static void Off(string eventName, Action listener)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] = (Action)events[eventName] - listener;
            if (events[eventName] == null)
                events.Remove(eventName);
        }
    }

    // Unsubscribe (With generic parameter)
    public static void Off<T>(string eventName, Action<T> listener)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] = (Action<T>)events[eventName] - listener;
            if (events[eventName] == null)
                events.Remove(eventName);
        }
    }

    // Emit (No parameters)
    public static void Emit(string eventName)
    {
        if (events.TryGetValue(eventName, out Delegate thisEvent))
        {
            (thisEvent as Action)?.Invoke();
        }
    }

    // Emit (With generic parameter)
    public static void Emit<T>(string eventName, T data)
    {
        if (events.TryGetValue(eventName, out Delegate thisEvent))
        {
            (thisEvent as Action<T>)?.Invoke(data);
        }
    }
}