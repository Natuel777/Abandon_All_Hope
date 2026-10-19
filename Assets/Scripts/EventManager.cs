using System.Collections.Generic;
using System;

public static class EventManager
{
    public delegate void MyEvents(params object[] parameters);
    private static Dictionary<string, MyEvents> _events = new();

    public static void Subscribe(string name, MyEvents method = null) 
    {
        if(_events.ContainsKey(name))
            _events[name] += method;
        
        else _events.Add(name, method);
    }

    public static void Unsubscribe(string name, MyEvents method) 
    {
        if (!_events.ContainsKey(name)) return;

        _events[name] -= method;

        if(_events[name] == null)
            _events.Remove(name);
    }

    public static void Clear()
    {
        _events.Clear();
    }

    public static void Trigger(string name, bool removeAfterTrigger = false, params object[] parameters) 
    {
        if(_events.ContainsKey(name))
            _events[name]?.Invoke(parameters);

        if(removeAfterTrigger) 
            _events.Remove(name);
    }
}
