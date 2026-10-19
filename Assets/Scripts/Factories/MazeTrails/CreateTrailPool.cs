    using System.Collections.Generic;
    using UnityEngine;

    public class CreateTrailPool : IPooleable<Trail>
    {
        private TrailFactory _trailFactory;
        private Transform _spawnPoint;
        private MazeMiser _currentMiser;
        private ObjectPool<Trail> _pool;

        public CreateTrailPool(TrailFactory factory)
        {
            _trailFactory = factory;

            if (_trailFactory == null)
            {
                Debug.LogError("CreateTrailPool: trail factory is null. Pool will not be created.");
                return;
            }

            StartPoolCreation();
        }

        public void StartPoolCreation()
        {
            _pool = new ObjectPool<Trail>(InstanceTrail, ObjON, ObjOFF, 8);
        }

        private Trail InstanceTrail()
        {
            if(_spawnPoint == null)
            {
                return _trailFactory.Create(Vector3.zero, Quaternion.identity);
            }

            return _trailFactory.Create(_spawnPoint.position, _spawnPoint.rotation);
        }

        public void ObjON(Trail obj)
        {
            if(!obj.gameObject.activeSelf)
                GameManager.Instance.ActiveOrDesactive(obj.gameObject, true);

            if(!MazeManager.Instance.mazeTrails.Contains(obj))
            {
                MazeManager.Instance.mazeTrails.Add(obj);
                Debug.Log("Trail añadido a la lista");
            }
        }

        public void ObjOFF(Trail obj)
        {
            TrailRenderer tr = obj.GetComponentInChildren<TrailRenderer>();
            
            if (tr != null) tr.Clear();

            if(obj.gameObject.activeSelf)
                GameManager.Instance.ActiveOrDesactive(obj.gameObject, false);

            if(MazeManager.Instance.mazeTrails.Contains(obj))
            {
                Debug.Log("Trail Removido de la lista");
                MazeManager.Instance.mazeTrails.Remove(obj);
            }
        }
        
        public void ReturnToPool(Trail x)
        {
            if(_pool != null) _pool.Return(x);
        }

        public void GetObjectFromPool()
        {
            if(_pool != null)
            {
                Trail x = _pool.Get();
                RefreshPosition(x);
                x.GetParent(_currentMiser);
            } 
        }

        public void RefreshPosition(Trail x)
        {
            if(_spawnPoint != null)
            {
                x.transform.position = _spawnPoint.position;
                x.transform.rotation = _spawnPoint.rotation;
            }
        }

        public void GetMiserData(MazeMiser miser)
        {
            Debug.Log("Getting miser data");
            _currentMiser = miser;
            _spawnPoint = _currentMiser.trailSpawnPoint;
        }
    }
