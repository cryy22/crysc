#region

using UnityEngine;

#endregion

namespace Crysc.Helpers
{
    public static class VectorExtensions
    {
        public static Vector2 Abs(this Vector2 vector)
        {
            return new Vector2(x: Mathf.Abs(vector.x), y: Mathf.Abs(vector.y));
        }

        public static Vector3 Abs(this Vector3 vector)
        {
            return new Vector3(x: Mathf.Abs(vector.x), y: Mathf.Abs(vector.y), z: Mathf.Abs(vector.z));
        }
    }
}
