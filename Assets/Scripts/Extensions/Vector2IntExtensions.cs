using UnityEngine;

namespace Extensions
{
    public static class Vector2IntExtensions
    {
        public static Vector2 ToVector2(this Vector2Int vector2Int)
        {
            return new Vector2(vector2Int.x, vector2Int.y);
        }

        public static Vector2Int Normalize(this Vector2Int vector2Int)
        {
            return Vector2Int.FloorToInt(vector2Int.ToVector2().normalized);
        }
    }
}