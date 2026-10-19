using UnityEngine;

public class FallDamage
{
    private float _threshold, _multiplier, _fallSpeed, _damage = 0f;
    private Health _health;
    private GroundCheck _groundCheck;
    private Rigidbody _rb;

    public FallDamage(Rigidbody rb, Health h, GroundCheck g, Transform transform, LayerMask mask)
    {
        _rb = rb;
        _health = h;
        _groundCheck = g;
        _threshold = -15f;
        _multiplier = 2f;
    }

    public void ArtificialFixedUpdate()
    {
        //Implementar que el multiplier aumente si fallSpeed supera cierto umbral
        if(_health.Life <= 0f) return;

        if (!_groundCheck.IsGrounded)
        {
            if(_rb.linearVelocity.y < _fallSpeed)
                _fallSpeed = _rb.linearVelocity.y;
        }
        else
        {
            if(_fallSpeed < _threshold)
            {
                _damage = Mathf.Abs(_fallSpeed) * _multiplier;
                
                if(_fallSpeed < _threshold * 1.3f)
                    _damage = Mathf.Abs(_fallSpeed) * _multiplier * 2f;

                _health.ReceiveDamageValue(_damage);
            }
            
            _fallSpeed = 0f;
        }
    }
}
