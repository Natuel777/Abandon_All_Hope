using UnityEngine;

public class NPCMovement
{
    //Provisional movimiento básico con waypoints para tutorial. Luego se expandirá
    public Transform waypoint = null;
    private Transform _transform;
    private float _rotationSpeed;

    public NPCMovement(Transform transform, float rotationSpeed)
    {
        _transform = transform;
        _rotationSpeed = rotationSpeed;
    }

    public void ArtificialUpdate()
    {
        if(waypoint == null) return;

        Move();
    }

    private void Move()
    {
        Vector3 direction = waypoint.position -_transform.position;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _transform.position += direction.normalized * 1f * Time.deltaTime;
        _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }
}
