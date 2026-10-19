using System;
using System.Collections.Generic;

public class ObjectiveManager
{
    public static ObjectiveManager Instance { get; private set; } = new ObjectiveManager();
    public Objective ActiveObjective { get; private set; }
    public List<Objective> CompletedObjectives { get; private set; } = new();
    public Action<Objective> OnObjectiveCreated;
    public Action<Objective> OnObjectiveCompleted;

    public void SetObjective(string text, bool iconEnabled = true)
    {
        ActiveObjective = new Objective(text, iconEnabled);
        OnObjectiveCreated?.Invoke(ActiveObjective);
    }

    public void CompleteObjective()
    {
        if (ActiveObjective == null) return;

        ActiveObjective.Complete();
        CompletedObjectives.Add(ActiveObjective);
        OnObjectiveCompleted?.Invoke(ActiveObjective);
        ActiveObjective = null;
    }

    public Objective GetActiveObjective() => ActiveObjective;
}

public class Objective
{
    public string Text { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IconEnabled { get; private set; }

    public Objective(string text, bool iconEnabled = true)
    {
        Text = text;
        IconEnabled = iconEnabled;
    }

    public void Complete()
    {
        IsCompleted = true;
    }
}