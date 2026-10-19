using System.Collections.Generic;
using System;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> _stock = new ();
    private Action<T> _On;
    private Action<T> _Off;
    private Func<T> _Factory;

    public ObjectPool(Func<T> Factory, Action<T> ObjOn, Action<T> ObjOff, int initialStock = 15)
    {
        _Factory = Factory;
        _On = ObjOn;
        _Off = ObjOff;

        for(int i=0; i < initialStock; i++)
        {
            var x = _Factory();
            // Skip null factory results
            if(x == null) continue;

            if(_Off != null) _Off(x);

            else x.gameObject.SetActive(false);

            _stock.Enqueue(x);
        }
    }

    public T Get(string type = "")
    {
        T x = null;

        if(type != "" && _stock.Count > 0)
        {
            //HardCode con cast
            if(_stock is Queue<Weight> weightStock)
            {
                foreach(Weight w in weightStock)
                {
                    if(w == null) continue;

                    if(w.Type == type)
                    {
                        x = (T)(MonoBehaviour)w;
                        _stock = QueueExtensions.RemoveItem<T>(_stock, x);
                        break;
                    }
                }
            }
        }

        if(x == null)
        {
            if(_stock.Count > 0 && type == "") x = _stock.Dequeue();

            else
            {
                if(_Factory != null) x = _Factory();

                else
                {
                    Debug.Log("ObjectPool.Get: No factory available to create instances and none in stock.");
                    return null;
                }
            }
        }

        if(x == null)
        {
            Debug.Log("ObjectPool.Get: returned null after search and factory creation.");
            return null;
        }

        if(_On != null) _On(x);

        else x.gameObject.SetActive(true);

        return x;
    }

    public void Return(T obj)
    {
        _Off(obj);

        if(!obj.gameObject.activeInHierarchy)
        {
            _stock.Enqueue(obj);
            Debug.Log("Retornado correctamente a pool");
        }

        else Debug.Log("No se pudo retornar el objeto al pool porque sigue activo en la jerarquía.");
    }
}
