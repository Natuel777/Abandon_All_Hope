using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 FixZeroCross(this Vector2 raw, Vector2 smooth)
    {
        Vector2 result = raw;

        if (raw.x != 0 && Mathf.Sign(raw.x) != Mathf.Sign(smooth.x))
            result.x = raw.x * 0.001f;

        if (raw.y != 0 && Mathf.Sign(raw.y) != Mathf.Sign(smooth.y))
            result.y = raw.y * 0.001f;

        return result;
    }
}
