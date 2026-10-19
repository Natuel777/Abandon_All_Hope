using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected EntityDataSO data;
    [SerializeField] protected float speed, maxSpeed;
}
