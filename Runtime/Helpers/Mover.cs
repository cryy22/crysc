using System.Collections;
using UnityEngine;

namespace Crysc.Helpers
{
    public static class Mover
    {
        public static IEnumerator Move(Transform transform, Vector3 end, float duration = 0.125f)
        {
            Vector3 start = transform.position;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / duration;
                transform.position = Vector3.Lerp(a: start, b: end, t: t);
                yield return null;
            }

            transform.position = end;
        }

        public static IEnumerator MoveLocal(Transform transform, Vector3 end, float duration = 0.125f)
        {
            yield return Move(transform: transform, end: transform.parent.TransformPoint(end), duration: duration);
        }
    }
}
