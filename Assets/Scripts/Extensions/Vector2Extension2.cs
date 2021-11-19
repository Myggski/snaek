using UnityEngine;

namespace Extensions
{
    public static class Vector2Extension2
    {
        public static Vector2 ToVector2Int(this Vector2 vector2)
        {
            return Vector2Int.FloorToInt(vector2);
        }
    }
}