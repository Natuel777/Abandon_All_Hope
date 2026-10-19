using UnityEngine;
using System.Collections.Generic;

public class ObjectivesUI : MonoBehaviour
{
    [SerializeField] private Transform _gridContainer;
    [SerializeField] private ObjectiveItem _itemPrefab;
    private Dictionary<Objective, ObjectiveItem> _visualItems = new();

    private void OnEnable()
    {
        ObjectiveManager.Instance.OnObjectiveCreated += AddObjectiveUI;
        ObjectiveManager.Instance.OnObjectiveCompleted += CompleteObjectiveUI;
    }

    private void OnDisable()
    {
        ObjectiveManager.Instance.OnObjectiveCreated -= AddObjectiveUI;
        ObjectiveManager.Instance.OnObjectiveCompleted -= CompleteObjectiveUI;
    }

    private void AddObjectiveUI(Objective obj)
    {
        var newItem = Instantiate(_itemPrefab, _gridContainer);
        newItem.Initialize(obj);
        _visualItems.Add(obj, newItem);
    }

    private void CompleteObjectiveUI(Objective obj)
    {
        if(_visualItems.TryGetValue(obj, out ObjectiveItem item)) item.MarkAsCompleted();
    }
}