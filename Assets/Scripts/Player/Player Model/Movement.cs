using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    private GroundCheck _groundCheck;
    private Rigidbody _rb; 
    private Dictionary<string, float> _speedTypes = new();

    //Jump
    private float _jumpForce, _jumpCoolDown, _airMultiplier, _timer = 0f;
    public bool readyToJump = true;

    //Crouch
    private float _initialScale, _crouchScale;
    private Transform _meshTransform;

    //Movement
    private bool _isSprinting, _isCrouching;
    private Vector3 _inputDirection;

    //Slope Movement
    private float _maxSlopeAngle = 40f;

    //Air Speed Control
    private float _currentForwardSpeed = 0f;
    private float _airBrakeSpeed = 0.15f; // Reducido para una frenada más suave
    private float _lastGroundedSpeed = 0f; // Para recordar la velocidad que teníamos al saltar
    private bool _wasGrounded = true;

    public Movement(float speed, float sprintSpeed, Rigidbody rb, Transform meshTransform, float jumpForce)
    {
        _speedTypes.Add("Regular", speed);
        _speedTypes.Add("Sprint", sprintSpeed);
        _speedTypes.Add("AirMultiplier", 1.4f);
        _speedTypes.Add("Crouch", speed * 0.5f);
        _rb = rb;
        _meshTransform = meshTransform;
        _jumpForce = jumpForce;

        _jumpCoolDown = 0.25f;
        _initialScale = _meshTransform.localScale.y;
        _crouchScale = _initialScale * 0.5f;
    }

    
    public void SetMovementInputs(float inputX, float inputY, Transform transform, bool sprint = false, bool crouch = false)
    {
        inputY = SlopeMovement(inputY);
        _inputDirection = (transform.right * inputX + transform.forward * _currentForwardSpeed).normalized;
        _isSprinting = sprint;
        _isCrouching = crouch;
    }


    public void Move()
    {
        UpdateJumpTimer();

        if(_inputDirection == Vector3.zero || _inputDirection.sqrMagnitude < 0.05f)
        {
            if(OnSlope()) 
            {
                Debug.Log("Slop idle");
                _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
            }

            return;
        } 

        float currentSpeed = _speedTypes["Regular"];

        if(_isSprinting && !_isCrouching)
            currentSpeed = _speedTypes["Sprint"];

        else if(_isCrouching)
            currentSpeed = _speedTypes["Crouch"];

        if(!_groundCheck.IsGrounded)
        {
            currentSpeed *= _speedTypes["AirMultiplier"];

            if(_rb.linearVelocity.z > 0f && _inputDirection.z < 0f)
            {
                float lerpedZ = Mathf.Lerp(_rb.linearVelocity.z, 0f, Time.fixedDeltaTime);
                Vector3 airVelocity = new Vector3(_rb.linearVelocity.x, _rb.linearVelocity.y, lerpedZ);
                _rb.linearVelocity = airVelocity;
            }
        }

        if(OnSlope())
        {
            Vector3 slopeMove = GetSlopeMoveDirection() * currentSpeed * Time.fixedDeltaTime;
            _rb.MovePosition(_rb.position + slopeMove); 
            Debug.Log("Sloping");
            return;
        }

        Vector3 flatMove = _inputDirection * currentSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + flatMove);
    }

    private bool OnSlope()
    {
        if (!_groundCheck.IsGrounded) return false;
    
        float angle = Vector3.Angle(_groundCheck.GroundHit.normal, Vector3.up);
        return angle > 0f && angle <= _maxSlopeAngle;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_inputDirection, _groundCheck.GroundHit.normal).normalized;
    }

    private float SlopeMovement(float inputY)
    {
        if(!_groundCheck.IsGrounded)
        {
            // Si acabamos de dejar el suelo, guardamos la velocidad inicial
            if (_groundCheck.IsGrounded != _wasGrounded)
            {
                _lastGroundedSpeed = inputY;
                _currentForwardSpeed = _lastGroundedSpeed;
                _wasGrounded = false;
            }

            if(inputY < 0)
            {
                // Frenado más suave usando curva exponencial
                float brakeFactor = Mathf.Pow(Time.deltaTime * _airBrakeSpeed, 0.7f);
                _currentForwardSpeed = Mathf.Lerp(_currentForwardSpeed, 0f, brakeFactor);
                inputY = Mathf.Max(0, _currentForwardSpeed); // Aseguramos que no vaya negativo
            }
            else if(inputY > 0)
            {
                // Permitimos acelerar pero con cierta limitación en el aire
                _currentForwardSpeed = Mathf.Lerp(_currentForwardSpeed, inputY * 0.8f, Time.deltaTime);
                inputY = _currentForwardSpeed;
            }
        }

        else
        {
            _wasGrounded = true;
            _currentForwardSpeed = inputY;
        }

        return inputY;
    }

    public void Jump(Transform transform)
    {
        if(readyToJump && _groundCheck.IsGrounded)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
            readyToJump = false;
        }
    }

    public void Crouch(bool crouchDown)
    {
        if(!_groundCheck.IsGrounded) return;

        CapsuleCollider collider = GameManager.Instance.player.GetComponent<CapsuleCollider>();

        if(crouchDown)
        {
            _meshTransform.localScale = new Vector3(_meshTransform.localScale.x, _crouchScale, _meshTransform.localScale.z);
            
            if(collider != null) collider.height *= 0.5f;

            _rb.AddForce(Vector3.down * 0.5f);
        }

        else
        {
            _meshTransform.localScale = new Vector3(_meshTransform.localScale.x, _initialScale, _meshTransform.localScale.z);
            
            if(collider != null) collider.height *= 2f;
        }
    }

    private void UpdateJumpTimer()
    {
        if(!readyToJump)
        {
            _timer += Time.deltaTime;

            if(_timer >= _jumpCoolDown) 
            {
                readyToJump = true;
                _timer = 0f;
            }
        }
    }

    public Movement GetGroundCheck(GroundCheck gc)
    {
        _groundCheck = gc;
        return this;
    }
}
