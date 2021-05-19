using UnityEngine;

namespace HQ
{
    static class Extensions
    {
        public static bool InRange(this Vector2Int range, int i)
        {
            return i > range.x && i < range.y;
        }
    }
}
