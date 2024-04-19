using UnityEngine;

namespace Crysc.Helpers
{
    public static class LocalSpaceConverter
    {
        public static Vector3 ConvertPosition(Transform from, Transform to, Vector3 position)
        {
            return to.InverseTransformPoint(from.TransformPoint(position));
        }

        public static Quaternion ConvertRotation(Transform from, Transform to, Quaternion rotation)
        {
            Vector3 fromEuler = rotation.eulerAngles;
            Vector3 toEuler = from.InverseTransformDirection(to.TransformDirection(fromEuler));
            return Quaternion.Euler(toEuler);
        }

        public static Vector3 ConvertScale(Transform from, Transform to, Vector3 scale)
        {
            return to.InverseTransformVector(from.TransformVector(scale));
        }

        public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
        {
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3(
                x: globalScale.x / transform.localScale.x,
                y: globalScale.y / transform.localScale.y,
                z: globalScale.z / transform.localScale.z
            );
        }
    }
}
