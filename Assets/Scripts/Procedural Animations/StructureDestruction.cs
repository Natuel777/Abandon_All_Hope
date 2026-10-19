using UnityEngine;
using System.Collections.Generic;

public class StructureDestruction : MonoBehaviour
{
    [SerializeField] private List<Rigidbody> _childrenRB = new();
    private bool _hasCollided = false;
    private float _timer = 6f;
    
    private void Start() 
    {
        foreach(Transform child in transform)
        {
            if(child.TryGetComponent<Rigidbody>(out Rigidbody rb))
                _childrenRB.Add(rb);
        }   
    }

    private void Update() 
    {
        if(!_hasCollided) return;

        _timer -= Time.deltaTime;

        if(_timer <= 0f) 
        {
            foreach(Rigidbody item2 in _childrenRB)
            {
                GameManager.Instance.DestroyComponent(item2);
            }

            GameManager.Instance.DestroyComponent(this);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.GetComponent<Collider>().CompareTag("Rock"))
        {
            BreakApart(transform.position);
        } 
    }

    private void BreakApart(Vector3 point)
    {
        Debug.Log("break");
        foreach(Rigidbody item in _childrenRB)
        {
            item.isKinematic = false;
            item.AddForce(new Vector3(Random.Range(-10f, 10f), 
                            Random.Range(-10f, 10f), 
                            Random.Range(-10f, 10f)), 
                            ForceMode.Impulse);
        }    

        _hasCollided = true;
    }
}
