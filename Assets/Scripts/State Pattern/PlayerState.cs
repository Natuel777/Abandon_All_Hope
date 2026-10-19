using UnityEngine;

[System.Serializable]
public class PlayerState
{
    public float health;
    public Vector3 position;

    public PlayerState(float health, Vector3 position)
    {
        this.health = health;
        this.position = position;
    }
}
