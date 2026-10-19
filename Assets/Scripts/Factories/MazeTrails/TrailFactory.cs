using System.Collections.Generic;
using UnityEngine;

public class TrailFactory : Factory<Trail>
{
    [SerializeField] private Trail _trailPrefab;

    public override Trail Create(Vector3 position, Quaternion rotation)
    {
        Trail x = Instantiate(_trailPrefab, position, rotation);
        return x;
    }
}
