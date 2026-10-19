using UnityEngine;

public class Trail : MonoBehaviour
{
    private float _timer = 0f;
    [SerializeField] private float _speed = 5f, _sphereRadius = 0.5f, _collisionSmooth = 10f;
    private bool _isPooled = false;
    private MazeMiser _parent;
    private Vector3 _lastPosition;
    private PointDirection _pointing;

    private void Start() 
    {
        _pointing = GameManager.Instance.playerClass.interaction.pointDirection;
    }

    private void OnEnable()
    {
        _isPooled = false;

        if(MazeManager.Instance != null && !MazeManager.Instance.mazeTrails.Contains(this))
        {
            MazeManager.Instance.mazeTrails.Add(this);
        }

        else Debug.Log("No se pudo meter a la lista este trail");
    }

    private void OnDisable()
    {
        if(MazeManager.Instance != null && MazeManager.Instance.mazeTrails.Contains(this))
            MazeManager.Instance.mazeTrails.Remove(this);
    }

    public void GetParent(MazeMiser parent)
    {
        _parent = parent;
    }

    public void ArtificialUpdate() 
    {
        if(_parent.pointTarget == Vector3.zero || _isPooled) return;

        Move();

        if(!_pointing.IsPointing)
            DistanceCounter();
    }

    private void Move()
    {
        Vector3 direction = (_parent.pointTarget - transform.position).normalized;
        float moveDist = _speed * Time.deltaTime;
        Vector3 nextPosition = transform.position;

        if(Physics.SphereCast(transform.position, _sphereRadius, direction, out RaycastHit hit, moveDist))
        {
            if(_pointing.IsPointing) nextPosition = hit.point - direction * _sphereRadius;

            else nextPosition = transform.position;
        }

        else nextPosition = transform.position + direction * moveDist;

        

        if(_pointing.IsPointing)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _sphereRadius);

            foreach(Collider col in hits)
            {
                if(!col.isTrigger)
                {
                    Vector3 dir = nextPosition - col.ClosestPoint(transform.position);
                    float dist = _sphereRadius - dir.magnitude;
    
                    if(dist > 0f)
                    {
                        Vector3 correction = dir.normalized * dist;
                        nextPosition = Vector3.Lerp(nextPosition, nextPosition + correction, Time.deltaTime * _collisionSmooth);
                    } 
                }
            }
        }

        transform.position = nextPosition;
    }

    private void DistanceCounter()
    {
        float distanceMoved = (transform.position - _lastPosition).magnitude;

        if(distanceMoved > 1f)
        {
            _timer = 0f;
            _lastPosition = transform.position;
        }

        else
        {
            _timer += Time.deltaTime;

            if(_timer >= 0.8f)
            {
                _parent = null;
                _isPooled = true;
                MazeManager.Instance.trailPoolManager.ReturnToPool(this);
            }
        }
    }
}
