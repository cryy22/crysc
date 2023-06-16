using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Crysc.Helpers
{
    public static class Mover
    {
        public static IEnumerator MoveTo(
            Transform transform,
            Vector3 end,
            float duration = 0.25f,
            bool isLocal = true
        )
        {
            Vector3 start = GetPosition(transform: transform, isLocal: isLocal);

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;

                SetPosition(
                    transform: transform,
                    position: Vector3.Lerp(a: start, b: end, t: t),
                    isLocal: isLocal
                );

                yield return null;
            }

            SetPosition(transform: transform, position: end, isLocal: isLocal);
        }

        public static IEnumerator MoveToSmoothly(
            Transform transform,
            Vector3 end,
            float duration = 0.25f,
            bool isLocal = true
        )
        {
            yield return (
                isLocal
                    ? transform.DOLocalMove(endValue: end, duration: duration)
                    : transform.DOMove(endValue: end, duration: duration)
            ).WaitForCompletion();
        }

        public static IEnumerator MoveSine(
            Transform transform,
            Vector3 delta,
            float duration = 0.25f,
            float period = 0.25f,
            bool isLocal = true
        )
        {
            Vector3 initial = GetPosition(transform: transform, isLocal: isLocal);
            Vector3 min = initial - delta;
            Vector3 max = initial + delta;

            var t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;

                float sineT = (Mathf.Sin((t / period) * 2 * Mathf.PI) / 2) + 0.5f;
                Vector3 newPosition = Vector3.Lerp(a: min, b: max, t: sineT);
                SetPosition(transform: transform, position: newPosition, isLocal: isLocal);

                yield return null;
            }
        }

        private static Vector3 GetPosition(Transform transform, bool isLocal)
        {
            return isLocal ? transform.localPosition : transform.position;
        }

        private static void SetPosition(Transform transform, Vector3 position, bool isLocal)
        {
            if (isLocal) transform.localPosition = position;
            else transform.position = position;
        }
    }
}
