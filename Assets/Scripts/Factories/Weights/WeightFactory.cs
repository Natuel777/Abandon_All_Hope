using System.Collections.Generic;
using UnityEngine;

public class WeightFactory : Factory<Weight>
{
    [SerializeField] private List<Weight> _weightTypes = new();
    private int _order = 0;
    private string _currentType = "";

    public override Weight Create(Vector3 position, Quaternion rotation)
    {
        Weight x = null;
        
        if(_currentType != "")
        {
            foreach(Weight w in _weightTypes)
            {
                if(w.Type == _currentType)
                {
                    x = Instantiate(w, position, rotation);
                }
            }
        }

        else
        {
            if(_order >= _weightTypes.Count) _order = 0;
        
            x = Instantiate(_weightTypes[_order], position, rotation);
            _order++;
        }

        _currentType = "";
        return x;
    }

    public void GetType(string type)
    {
        _currentType = type;
    }
}
