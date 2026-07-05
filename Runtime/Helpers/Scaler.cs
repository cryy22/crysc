using System.Collections;
using Crysc.Presentation.Arrangements;
using PrimeTween;
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
            yield return Tween
                .Scale(transform, endValue: end, duration: duration, ease: Ease.Linear)
                .ToStoppableYield();
        }

        public static void ScaleToStep(
            Transform transform,
            Vector3 start,
            Vector3 end,
            float t,
            Easings.Enum easing = Easings.Enum.Linear
        )
        {
            transform.localScale = Vector3.LerpUnclamped(a: start, b: end, t: Easings.Ease(t: t, easing: easing));
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

            transform.localScale = end;
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
