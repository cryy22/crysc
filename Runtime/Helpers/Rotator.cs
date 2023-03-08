using System.Collections;
using UnityEngine;

namespace Crysc.Helpers
{
    public static class Rotator
    {
        public static IEnumerator RotateTo(
            Transform transform,
            Vector3 end,
            float duration = 0.25f,
            bool isLocal = true
        )
        {
            Vector3 start = GetRotation(transform: transform, isLocal: isLocal);

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;

                SetRotation(
                    transform: transform,
                    rotation: Vector3.Lerp(a: start, b: end, t: t),
                    isLocal: isLocal
                );

                yield return null;
            }

            SetRotation(transform: transform, rotation: end, isLocal: isLocal);
        }

        public static IEnumerator RotateToSmoothly(
            Transform transform,
            Vector3 end,
            float duration = 0.25f,
            bool isLocal = true
        )
        {
            Vector3 start = GetRotation(transform: transform, isLocal: isLocal);
            Vector3 delta = end - start;

            yield return RotateSine(
                transform: transform,
                delta: delta,
                duration: duration,
                period: duration * 4,
                isLocal: isLocal
            );

            SetRotation(transform: transform, rotation: end, isLocal: isLocal);
        }

        public static IEnumerator RotateSine(
            Transform transform,
            Vector3 delta,
            float duration = 0.25f,
            float period = 0.25f,
            bool isLocal = true
        )
        {
            Vector3 initial = GetRotation(transform: transform, isLocal: isLocal);
            Vector3 min = initial - delta;
            Vector3 max = initial + delta;

            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;

                float sineT = (Mathf.Sin((t / period) * 2 * Mathf.PI) / 2) + 0.5f;
                Vector3 newRotation = Vector3.Lerp(a: min, b: max, t: sineT);
                SetRotation(transform: transform, rotation: newRotation, isLocal: isLocal);

                yield return null;
            }
        }

        private static Vector3 GetRotation(Transform transform, bool isLocal)
        {
            return isLocal ? transform.localRotation.eulerAngles : transform.rotation.eulerAngles;
        }

        private static void SetRotation(Transform transform, Vector3 rotation, bool isLocal)
        {
            if (isLocal) transform.localRotation = Quaternion.Euler(rotation);
            else transform.rotation = Quaternion.Euler(rotation);
        }
    }
}
