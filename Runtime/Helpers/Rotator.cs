#region

using System.Collections;
using Crysc.Presentation.Arrangements;
using PrimeTween;
using UnityEngine;

#endregion

namespace Crysc.Helpers
{
    public static class Rotator
    {
        public static AnimationHandle RotateTo(
            Transform transform,
            Vector3 end,
            float duration = 0.25f,
            bool isLocal = true
        )
        {
            Vector3 start = GetRotation(transform: transform, isLocal: isLocal);

            return new AnimationHandle(
                Tween.Custom(
                    target: transform,
                    startValue: 0f,
                    endValue: 1f,
                    duration: duration,
                    onValueChange: (target, t) => SetRotation(
                        transform: target,
                        rotation: Vector3.Lerp(a: start, b: end, t: t),
                        isLocal: isLocal
                    )
                )
            );
        }

        public static void RotateToStep(
            Transform transform,
            Quaternion start,
            Quaternion end,
            float t,
            int rotations = 0,
            bool isLocal = true,
            Easings.Enum easings = Easings.Enum.Linear
        )
        {
            float easedT = Easings.Ease(t: t, easing: easings);
            Quaternion rotation = Quaternion.LerpUnclamped(a: start, b: end, t: easedT);
            rotation *= Quaternion.Euler(x: 0, y: 0, z: 360 * -rotations * easedT);
            if (isLocal)
                transform.localRotation = rotation;
            else
                transform.rotation = rotation;
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

        public static AnimationHandle RotateToSmoothly(
            Transform transform,
            Quaternion end,
            float duration = 0.25f,
            bool isLocal = true
        )
        {
            return new AnimationHandle(
                isLocal
                    ? Tween.LocalRotation(target: transform, endValue: end, duration: duration)
                    : Tween.Rotation(target: transform, endValue: end, duration: duration)
            );
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

                float sineT = Mathf.Sin(t / period * 2 * Mathf.PI) / 2 + 0.5f;
                Vector3 newRotation = Vector3.Lerp(a: min, b: max, t: sineT);
                SetRotation(transform: transform, rotation: newRotation, isLocal: isLocal);

                yield return null;
            }
        }

        public static Vector3 GetRotation(Transform transform, bool isLocal)
        {
            return isLocal ? transform.localRotation.eulerAngles : transform.rotation.eulerAngles;
        }

        public static void SetRotation(Transform transform, Vector3 rotation, bool isLocal)
        {
            if (isLocal) transform.localRotation = Quaternion.Euler(rotation);
            else transform.rotation = Quaternion.Euler(rotation);
        }
    }
}
