using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FPCamera : ProjectCamera
{
    [SerializeField, Range(1f, 100f)] private float _XSensibility, _YSensibility;
    private float _XRotation, _YRotation;
    private Vector2 _mouseInputs = Vector2.zero;

    [Header("HeadBob")]
    Vector3 headBobOffset = Vector3.zero;
    [Range(0.01f, 0.2f), SerializeField] private float _amount, _sprintAmount;
    [Range(1f, 15f), SerializeField] private float _frecuency, _sprintFrecuency;
    private float _smooth = 6f;     
    private Vector3 _shakeOffset = Vector3.zero;

    [Header("Depth of Field")]    
    [SerializeField] private Volume _postVolume;
    [SerializeField] private DepthOfField _depthOfField;

    [Header("Waypoint Marker")]
    public WaypointMarker waypointMarker;

    [Header("Player Body")]
    [SerializeField] private Transform _playerBody;

    protected override void Start()
    {
        base.Start();

        if(_playerBody == null)
            _playerBody = GetComponentInParent<Player>()?.transform
                          ?? GameManager.Instance?.player.transform;

        if(_postVolume == null) return;

        _postVolume.profile.TryGet<DepthOfField>(out _depthOfField);
        EventManager.Subscribe("OnTutorialEnded", waypointMarker.SetTarget);
    }

    private void LateUpdate() 
    {
        if(cameraPosition == null || Time.timeScale == 0) return;
        
        transform.position = cameraPosition.position + headBobOffset + _shakeOffset;
        waypointMarker.ArtificialUpdate();
    }

    public void GetInputs(float inputX, float inputY)
    {
        if(cameraPosition == null || Time.timeScale == 0) return;

        float mouseX = inputX * _XSensibility;
        float mouseY = inputY * _YSensibility;
        _mouseInputs = new Vector2(mouseX, mouseY);
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        if(cameraPosition == null || Time.timeScale == 0) return;
        _YRotation += _mouseInputs.x;
        _XRotation -= _mouseInputs.y;
        _XRotation = Mathf.Clamp(_XRotation, -90f, 90f);
        transform.rotation = Quaternion.Euler(_XRotation, _YRotation, 0);
        if(_playerBody != null) _playerBody.rotation = Quaternion.Euler(0, _YRotation, 0);
    }

    public void StartHeadBob(float inputMagnitude, bool isSprinting = false)
    {
        if(cameraPosition == null || Time.timeScale == 0) return;

        if(inputMagnitude > 0.1f)
        {
            if(isSprinting)
            {
                ApplyHeadBob(_sprintFrecuency, _sprintAmount);
                //ApplyBlur(true);
            }

            else
            {
                ApplyHeadBob(_frecuency, _amount);
                //ApplyBlur(false);
            }
        }

        else
        {
            headBobOffset = Vector3.Lerp(headBobOffset, Vector3.zero, _smooth * Time.deltaTime);
            //ApplyBlur(false);
        }
    }

    private void ApplyHeadBob(float freq, float amt)
    {
        if(cameraPosition == null || Time.timeScale == 0) return;

        headBobOffset.y = Mathf.Lerp(headBobOffset.y, 
                Mathf.Sin(Time.time * freq) * amt * 1.4f, 
                _smooth * Time.deltaTime);
        headBobOffset.x = Mathf.Lerp(headBobOffset.x, 
                Mathf.Cos(Time.time * freq / 2f) * amt * 1.6f, 
                _smooth * Time.deltaTime);
    }

    private void ApplyBlur(bool value)
    {
        if(cameraPosition == null || Time.timeScale == 0) return;

        if (_depthOfField == null)
        {
            Debug.Log("ojete");
            return;
        }

        if(value)
        {
            _depthOfField.focusDistance.value = 0.5f;
            _depthOfField.aperture.value = 32f;
        }
        else
        {
            _depthOfField.focusDistance.value = 2.8f;
            _depthOfField.aperture.value = 5.6f;
        }
    }

    public void SetShakeOffset(Vector3 offset)
    {
        if(cameraPosition == null || Time.timeScale == 0) return;
        _shakeOffset = offset;
    }
}


