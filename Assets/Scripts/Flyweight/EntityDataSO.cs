using UnityEngine;

[CreateAssetMenu(fileName = "EntityDataSO", menuName = "ScriptableObjects/EntityData")]
public class EntityDataSO : ScriptableObject 
{
    public float maxLife = 100f;
    public float NPCRotationSpeed = 5f;
}

