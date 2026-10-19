using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weight : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _detectionRadius, _pushForce;
    private float _timer, _stuckThreshold = 0.5f, _stuckTimeLimit = 5f;
    private Vector3 _lastPosition;
    private bool _isMoving = true, _hasReturnedToPool = false;
    private Vector3 _direction, _initialDirection = Vector3.forward;
    [SerializeField] private string _type;
    public string Type => _type;

    private void Start() 
    {
        if(!MazeManager.Instance.weightInstances.Contains(this))
        {
            MazeManager.Instance.weightInstances.Add(this);
        }

        EventManager.Subscribe("OnBalanceCompleted");
        _direction = _initialDirection;
        ResetPhysics();
    }

    public void ArtificialUpdate()
    {
        if(_rb.isKinematic || _hasReturnedToPool) return;

        if (_isMoving) PushForward();

        if(gameObject.name.StartsWith("BG")) return;

        float distanceMoved = (transform.position - _lastPosition).magnitude;

        if(distanceMoved > _stuckThreshold)
        {
            _timer = 0f;
            _lastPosition = transform.position;
        }

        else
        {
            _timer += Time.deltaTime;

            if(_timer >= _stuckTimeLimit)
            {
                AudioManager.Instance.PlaySFX(new AudioClipData("RockBreak"));
                Collider[] overlapped = Physics.OverlapSphere(transform.position, _detectionRadius);
                
                foreach(Collider col in overlapped)
                {
                    if(col.TryGetComponent<NeutralNPC>(out NeutralNPC miser) && miser.isMazeMiser)
                    {
                        miser.mazeMiser.SetListening(true);
                        miser.SetInteractable(true);
                    }
                }

                Despawn();
            }
        }
    }

    public void Despawn()
    {
        MazeManager.Instance.weightPoolManager.ReturnToPool(this);
        _hasReturnedToPool = true;
    }

    private void Redirect()
    {
        StartCoroutine(PauseMovement());
    }

    private void PushForward()
    {
        _rb.AddForce(_direction * _pushForce, ForceMode.Force);
    }

    public void ResetPhysics()
    {
        _hasReturnedToPool = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _direction = _initialDirection;
        _isMoving = true;
        _timer = 0f;
        _lastPosition = transform.position;
        _rb.Sleep();
    }

    private IEnumerator PauseMovement()
    {
        _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, Vector3.zero, Time.deltaTime * 8f);
        _rb.angularVelocity = Vector3.Lerp(_rb.angularVelocity, Vector3.zero, Time.deltaTime * 8f);
        _isMoving = false;
        yield return new WaitForSeconds(1.5f);
        _isMoving = true;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(_rb.isKinematic) return;
        
        if (other.TryGetComponent<NeutralNPC>(out NeutralNPC miser))
        {
            if(!miser.isMazeMiser) return;

            _direction = miser.mazeMiser.currentPushDirection;
            miser.mazeMiser.SetListening(false);
            miser.SetInteractable(false);
            Redirect();
            return;
        }

        if (other.TryGetComponent<BackgroundMiser>(out BackgroundMiser bgMiser))
        {
            _direction = bgMiser.pushDirection;
            Redirect();
            return;
        }

        if (other.TryGetComponent<Balance>(out Balance balance))
        {
            balance.SetWeight(this);
            return;
        }

        if(other.TryGetComponent<Hole>(out Hole hole))
        {
            hole.LoseWeight(this);
            return;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.TryGetComponent<NeutralNPC>(out NeutralNPC miser))
        {
            if(!miser.isMazeMiser) return;

            miser.mazeMiser.SetListening(true);
            miser.SetInteractable(true);

            if(miser.gameObject.name == "Tutorial Miser" && miser.movement != null)
            {
                miser.movement.waypoint = null;
            }
            
            return;
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
