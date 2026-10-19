using System.Collections.Generic;
using UnityEngine;

public class CreateWeightPool : IPooleable<Weight>
{
    public WeightFactory _weightFactory;
    public Transform _spawnPoint;
    private ObjectPool<Weight> _pool;
    private string _currentType;

    public CreateWeightPool(WeightFactory factory, Transform spawnPoint)
    {
        _weightFactory = factory;
        _spawnPoint = spawnPoint;

        if(_weightFactory == null)
        {
            Debug.LogError("CreateWeightPool: weight factory is null. Pool will not be created.");
            return;
        }

        if(_spawnPoint == null)
        {
            Debug.LogError("CreateWeightPool: spawn point is null. Pool will not be created.");
            return;
        }

        StartPoolCreation();

        // Ensure we don't subscribe the same handler multiple times
        EventManager.Unsubscribe("PoolWeight", OnPoolWeight);
        EventManager.Subscribe("PoolWeight", OnPoolWeight);
    }

    private void OnPoolWeight(params object[] args){GetObjectFromPool();}

    public void StartPoolCreation(){_pool = new ObjectPool<Weight>(InstanceWeight, ObjON, ObjOFF, 5);}

    private Weight InstanceWeight()
    {
        if(_currentType != "") _weightFactory.GetType(_currentType);
            
        return _weightFactory.Create(_spawnPoint.position, _spawnPoint.rotation);
    }

    public void ObjON(Weight obj)
    {
        if(obj == null) return;

        if(obj.gameObject != null && !obj.gameObject.activeSelf)
        {
            if(GameManager.Instance != null)
                GameManager.Instance.ActiveOrDesactive(obj.gameObject, true);

            else
                obj.gameObject.SetActive(true);
        }

        obj.ResetPhysics();

        if(MazeManager.Instance != null)
        {
            if(MazeManager.Instance.weightInstances == null)
                MazeManager.Instance.weightInstances = new System.Collections.Generic.List<Weight>();

            if(!MazeManager.Instance.weightInstances.Contains(obj))
                MazeManager.Instance.weightInstances.Add(obj);
        }

    }

    public void ObjOFF(Weight obj)
    {
        if(obj == null) return;

        if(obj.gameObject != null && obj.gameObject.activeSelf)
        {
            if(GameManager.Instance != null)
                GameManager.Instance.ActiveOrDesactive(obj.gameObject, false);

            else obj.gameObject.SetActive(false);
        }

        if(MazeManager.Instance != null && MazeManager.Instance.weightInstances != null && MazeManager.Instance.weightInstances.Contains(obj))
            MazeManager.Instance.weightInstances.Remove(obj);
    }
    
    public void ReturnToPool(Weight x) {if(_pool != null) _pool.Return(x);}

    public void GetObjectFromPool()
    {
        if(_pool != null)
        {
            Weight x = _pool.Get(_currentType);
            if (x == null)
            {
                Debug.Log("CreateWeightPool.GetObjectFromPool: pool returned null weight instance.");
                return;
            }

            RefreshPosition(x);
        }
    }

    public void RefreshPosition(Weight x)
    {
        if(x == null)
        {
            Debug.Log("CreateWeightPool.RefreshPosition called with null weight.");
            return;
        }

        if(_spawnPoint != null)
        {
            x.transform.position = _spawnPoint.position;
            x.transform.rotation = _spawnPoint.rotation;
            x.ResetPhysics();
        }
    }

    public void GetType(string type) {_currentType = type;}
}
