using UnityEngine;
//Arreglar
public class TutorialBalanceAnimation : MonoBehaviour
{
    public TutorialBalanceAnimation Instance { get; private set; }
    private GameObject _weightObject;
    [SerializeField] private Transform _targetPosition, _weightPosition;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        
        else
        {
            GameManager.Instance.DestroyObject(Instance.gameObject);
            Instance = this;
        }

        EventManager.Subscribe("OnTutorialEnded", GetWeight);
        EventManager.Subscribe("OnTutorialEnded", x => MoveToTarget());
    }

    private void GetWeight(params object[] parameters)
    {
        foreach(var item in parameters)
        {
            if(item is Weight weight)
            {
                _weightObject = weight.gameObject;
                _weightObject.transform.position = _weightPosition.position;
                GameManager.Instance.DestroyComponent(weight);
                _weightObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }

    private void MoveToTarget()
    {
        transform.position = new Vector3(transform.position.x, 
                                        Mathf.MoveTowards(transform.position.y, 
                                        _targetPosition.position.y, 
                                        Time.deltaTime * 0.2f),
                                        transform.position.z);

        if(Vector3.Distance(_weightObject.transform.position, _targetPosition.position) < 0.001f)
            GameManager.Instance.DestroyComponent(this);
    }
}
