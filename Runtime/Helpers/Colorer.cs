using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Crysc.Helpers
{
    public static class Colorer
    {
        public static IEnumerator ColorTo(
            Image image,
            Color end,
            float duration = 0.25f
        )
        {
            Color start = image.color;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;

                image.color = Color.Lerp(a: start, b: end, t: t);

                yield return null;
            }

            image.color = end;
        }

        public static IEnumerator ColorToSmoothly(
            Image image,
            Color end,
            float duration = 0.25f
        )
        {
            Color start = image.color;
            Color delta = end - start;

            yield return ColorSine(
                image: image,
                delta: delta,
                duration: duration,
                period: duration * 4
            );

            image.color = end;
        }

        public static IEnumerator ColorSine(
            Image image,
            Color delta,
            float duration = 0.25f,
            float period = 0.25f
        )
        {
            Color start = image.color;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;

                image.color = start + (delta * Mathf.Sin((t * Mathf.PI * 2) / period));

                yield return null;
            }

            image.color = start + delta;
        }
    }
}
