using UnityEngine;

public class GroundCheck
{
    private LayerMask _groundMask;
    public bool IsGrounded {get; private set;}
    public RaycastHit GroundHit {get; private set;}
    private float _rayLength = 1.1f;
    private Transform _transform;
    private CameraShake _cameraShake;

    public GroundCheck(Transform transform, LayerMask groundMask, CameraShake cameraShake)
    {
        _transform = transform;
        _groundMask = groundMask;
        _cameraShake = cameraShake;
    }

    public void ArtificialUpdate()
    {
        if(Physics.Raycast(_transform.position, Vector3.down, out RaycastHit hit, _rayLength, _groundMask))
        {
            GroundHit = hit;

            if(!IsGrounded)
            {
                IsGrounded = true;
                //_cameraShake.ShakeCamera(0.1f, 0.02f);
            }
        }

        else
        {
            IsGrounded = false;
            GroundHit = default;
        } 
    }    
}
