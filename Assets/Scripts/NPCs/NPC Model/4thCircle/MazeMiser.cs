using System.Collections;
using UnityEngine;

public class MazeMiser : IInteractable
{
    public bool listeningOrders = false;
    private bool _trialPooled;
    public Vector3 pendingPushDirection, currentPushDirection, pointTarget;
    private NPCRotation _rotation;
    private Transform _transform;
    public InteractableOutline interactable => null;
    public Transform trailSpawnPoint;

    public MazeMiser(NPCRotation rotation, Transform transform, Transform trailSpawn) 
    {
        pendingPushDirection = transform.forward;
        currentPushDirection = transform.forward;

        _transform = transform;
        _rotation = rotation;
        trailSpawnPoint = trailSpawn;
    }

    public void Interact()
    {
        SetListening(true);
    }

    public void SetListening(bool value)
    {
        listeningOrders = value;
        MazeManager.Instance.trailPoolManager.GetMiserData(this);
        
    }

    public void GetDirection(Vector3 initialDir)
    {
        if(!listeningOrders) return;
        
        Vector3 direction = initialDir - _transform.position;
        Vector3[] candidates = {Vector3.forward, Vector3.back, Vector3.right, Vector3.left};
        pendingPushDirection = Vector3.zero;
        float bestDot = -Mathf.Infinity;

        foreach(Vector3 candidate in candidates)
        {
            float dot = Vector3.Dot(direction.normalized, candidate);

            if(dot > bestDot)
            {
                bestDot = dot;
                pendingPushDirection = candidate;
            }
        }
    }

    public void ConfirmDirection()
    {
        currentPushDirection = pendingPushDirection;
        listeningOrders = false;
        _trialPooled = false;
    }

    public void SetLookAt(Vector3 target)
    {
        if(!_trialPooled)
        {
            MazeManager.Instance.trailPoolManager.GetObjectFromPool();
            _trialPooled = true;
        }

        pointTarget = new Vector3(target.x, _transform.position.y, target.z);
        _rotation.GetTarget(pointTarget);
    }

    public void SetInteractable(bool value)
    {
        //Not needed for MazeMiser
    }
}
