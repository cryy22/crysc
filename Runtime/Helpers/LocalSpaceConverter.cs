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
    }
}
