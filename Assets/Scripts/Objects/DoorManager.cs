using UnityEngine;
using System.Collections;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private Transform _door;
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private float _doorSpeed;
    private Vector3 _initialPosition;
    private int _totalRequiredWeights = 6, _currentWeights = 0;
    private bool _isMoving = false;
    public static DoorManager Instance {get; private set;}

    private void Awake() 
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() 
    {
        _initialPosition = _door.position;
    }

    public void RegisterCorrectWeight()
    {
        _currentWeights++;
        float t = (float)_currentWeights / _totalRequiredWeights;
        Vector3 nextPos = Vector3.Lerp(_initialPosition, _targetPosition, t);
        
        if(!_isMoving)
        {
            AudioManager.Instance.PlaySFX(new AudioClipData("GateMoving"));
            StartCoroutine(MoveDoorTo(nextPos));
        } 
    }

    private IEnumerator MoveDoorTo(Vector3 newPos)
    {

        _isMoving = true;

        //Cambiarlo dsp a Magnitude
        while(Mathf.Abs(_door.position.y - newPos.y) > 0.05f)
        {
            float newY = Mathf.MoveTowards(_door.position.y, newPos.y, _doorSpeed * Time.deltaTime);
            _door.position = new Vector3(_door.position.x, newY, _door.position.z);
            yield return null;
        }

        _door.position = newPos;
        _isMoving = false;

        if (_currentWeights >= _totalRequiredWeights)
        {
            Debug.Log("🚪 Door fully opened!");
            MazeManager.Instance.data.levelFinished = true;
        }
    }
}
