using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct WeightType
{
    public string type;
    public int requiredAmount;
}

public class Balance : MonoBehaviour
{
    [SerializeField] private List<WeightType> _requirements = new();
    [SerializeField] private List<GameObject> _woodenPosts = new();
    private Dictionary<string, int> _currentAmounts = new();
    
    private void Start() 
    {
        foreach(WeightType item in _requirements)
        {
            _currentAmounts[item.type] = 0;
        }
    }

    public void SetWeight(Weight weight)
    {
        if(!_currentAmounts.ContainsKey(weight.Type))
        {
            weight.Despawn();
            return;
        }

        if(_currentAmounts[weight.Type] < _requirements.Find(r => r.type == weight.Type).requiredAmount)
        {
            foreach(GameObject post in _woodenPosts)
            {
                if(!post.activeInHierarchy || post == null) continue;

                if(post.name.Contains(weight.Type))
                {
                    GameManager.Instance.DestroyObject(post);
                    break;
                }
            }

            _currentAmounts[weight.Type]++;
            weight.Despawn();
            DoorManager.Instance.RegisterCorrectWeight();
        }

        else weight.Despawn();
    }
}
