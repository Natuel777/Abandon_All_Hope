using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRotation
{
    private Transform _transform;
    public Vector3 target = Vector3.zero;
    private float _rotationSpeed, _timer, _timerMaxValue;

    public NPCRotation(Transform t, float rotationSpeed)
    {
        _transform = t;
        _rotationSpeed = rotationSpeed;

        _timerMaxValue = 5f;
        _timer = _timerMaxValue;
    }

    public void ArtificialUpdate()
    {
        Turn();
        _timer -= Time.deltaTime;
    }

    private void Turn()
    {
        Vector3 direction = target - _transform.position;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        if(_timer <= 0f)
        {
            target = Vector3.zero;
            _timer = _timerMaxValue;
            Debug.Log("Vuelvo a zero");
        }
    }

    public NPCRotation GetTarget(Vector3 v)
    {
        target = v;
        return this;
    }
}
