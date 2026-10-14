using UnityEngine;

public static class Vector2IntExtensions
{
    public static Vector2Int ToVector2Int(this Vector2 v)
    {
        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    }


    public static Vector2Int Normalized(this Vector2Int v)
    {
        return new Vector2Int(
            v.x == 0 ? 0 : (v.x > 0 ? 1 : -1),
            v.y == 0 ? 0 : (v.y > 0 ? 1 : -1)
        );
    }
}