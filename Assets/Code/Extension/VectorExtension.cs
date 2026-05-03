using UnityEngine;

namespace Droids.Code.Extension
{
    public static class VectorExtension
    {
        public static Vector2 ToVector2(this Vector3 vector) => new(vector.x, vector.y);
        
        public static Vector3 ToVector3(this Vector2 vector) => new Vector3(vector.x, vector.y, 0f);

        public static Vector3 ToVector3(this Vector2? vector) => new Vector3(vector.Value.x, vector.Value.y, 0f);
    }
}
