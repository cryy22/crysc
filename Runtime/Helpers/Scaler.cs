using System.Collections;
using UnityEngine;

namespace Crysc.Helpers
{
    public static class Scaler
    {
        public static IEnumerator ScaleTo(
            Transform transform,
            Vector3 end,
            float duration = 0.25f
        )
        {
            Vector3 start = transform.localScale;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;
                transform.localScale = Vector3.Lerp(a: start, b: end, t: t);

                yield return null;
            }

            transform.localScale = end;
        }

        public static IEnumerator ScaleToSmoothly(
            Transform transform,
            Vector3 end,
            float duration = 0.25f
        )
        {
            Vector3 start = transform.localScale;
            Vector3 delta = end - start;

            yield return ScaleSine(
                transform: transform,
                delta: delta,
                duration: duration,
                period: duration * 4
            );
        }

        public static IEnumerator ScaleSine(
            Transform transform,
            Vector3 delta,
            float duration = 0.25f,
            float period = 0.25f
        )
        {
            Vector3 initial = transform.localScale;
            Vector3 min = initial - delta;
            Vector3 max = initial + delta;

            var t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;

                float sineT = (Mathf.Sin((t / period) * 2 * Mathf.PI) / 2) + 0.5f;
                Vector3 newScale = Vector3.Lerp(a: min, b: max, t: sineT);
                transform.localScale = newScale;

                yield return null;
            }
        }
    }
}
