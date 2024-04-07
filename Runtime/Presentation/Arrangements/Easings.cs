using System;
using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    public abstract class Easings
    {
        public enum Enum
        {
            Linear,
            EaseInOutSine,
            EaseOutCubic,
            EaseInOutCubic,
            EaseOutElastic,
            EaseInBack,
            EaseOutBack,
            EaseInOutBack,
        }

        private const float _c1 = 1.70158f;
        private const float _c2 = _c1 * 1.525f;
        private const float _c3 = _c1 + 1;
        private const float _c4 = 2 * Mathf.PI / 3;

        public static float Ease(float t, Enum easing)
        {
            return easing switch
            {
                Enum.Linear         => Linear(t),
                Enum.EaseInOutSine  => EaseInOutSine(t),
                Enum.EaseOutCubic   => EaseOutCubic(t),
                Enum.EaseInOutCubic => EaseInOutCubic(t),
                Enum.EaseOutElastic => EaseOutElastic(t),
                Enum.EaseInBack     => EaseInBack(t),
                Enum.EaseOutBack    => EaseOutBack(t),
                Enum.EaseInOutBack  => EaseInOutBack(t),
                _ => throw new ArgumentOutOfRangeException(
                    paramName: nameof(easing),
                    actualValue: easing,
                    message: "no ease function for this easing"
                ),
            };
        }

        public static float Unease(float x, Enum easing)
        {
            return easing switch
            {
                Enum.EaseInOutSine  => UneaseInOutSine(x),
                Enum.EaseInOutCubic => UneaseInOutCubic(x),
                _ => throw new ArgumentOutOfRangeException(
                    paramName: nameof(easing),
                    actualValue: easing,
                    message: "no unease function for this easing"
                ),
            };
        }

        private static float Linear(float t) { return t; }

        private static float EaseInOutSine(float t) { return -(Mathf.Cos(Mathf.PI * t) - 1) / 2; }

        private static float UneaseInOutSine(float x) { return Mathf.Acos(-2 * x + 1) / Mathf.PI; }

        private static float EaseOutCubic(float t) { return 1 - Mathf.Pow(f: 1 - t, p: 3); }

        private static float EaseInOutCubic(float t)
        {
            return t < 0.5f
                ? 4f * t * t * t
                : 1f - Mathf.Pow(f: -2f * t + 2f, p: 3f) / 2f;
        }

        private static float UneaseInOutCubic(float x)
        {
            return x < 0.5f
                ? Mathf.Pow(f: x / 4, p: 1f / 3f)
                : -(Mathf.Pow(f: -2 * x + 2, p: 1f / 3f) - 2) / 2f;
        }

        private static float EaseOutElastic(float t)
        {
            return t switch
            {
                0 => 0,
                1 => 1,
                _ => Mathf.Pow(f: 2, p: -10 * t) * Mathf.Sin((float) ((t * 10 - 0.75) * _c4)) + 1,
            };
        }

        private static float EaseInBack(float t) { return _c3 * t * t * t - _c1 * t * t; }

        private static float EaseOutBack(float t)
        {
            return 1 + _c3 * Mathf.Pow(f: t - 1, p: 3) + _c1 * Mathf.Pow(f: t - 1, p: 2);
        }

        private static float EaseInOutBack(float t)
        {
            return t < 0.5f
                ? Mathf.Pow(f: 2 * t, p: 2) * ((_c2 + 1) * 2 * t - _c2) / 2
                : (Mathf.Pow(f: 2 * t - 2, p: 2) * ((_c2 + 1) * (t * 2 - 2) + _c2) + 2) / 2;
        }
    }
}
