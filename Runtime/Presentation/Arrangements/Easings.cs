using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    public abstract class Easings
    {
        public enum Enum
        {
            Linear,
            EaseOutElastic,
            EaseOutBack,
            EaseInOutBack,
        }

        private const float _c1 = 1.70158f;
        private const float _c2 = _c1 * 1.525f;
        private const float _c3 = _c1 + 1;
        private const float _c4 = (2 * Mathf.PI) / 3;

        public static float Ease(float t, Enum easing)
        {
            return easing switch
            {
                Enum.EaseOutElastic => EaseOutElastic(t),
                Enum.EaseInOutBack  => EaseInOutBack(t),
                Enum.EaseOutBack    => EaseOutBack(t),
                Enum.Linear         => Linear(t),
                _                   => Linear(t),
            };
        }

        private static float Linear(float t) { return t; }

        private static float EaseOutElastic(float t)
        {
            return t switch
            {
                0 => 0,
                1 => 1,
                _ => (Mathf.Pow(f: 2, p: -10 * t) * Mathf.Sin((float) (((t * 10) - 0.75) * _c4))) + 1,
            };
        }

        private static float EaseInOutBack(float t)
        {
            return t < 0.5
                ? (Mathf.Pow(f: 2 * t, p: 2) * (((_c2 + 1) * 2 * t) - _c2)) / 2
                : ((Mathf.Pow(f: (2 * t) - 2, p: 2) * (_c2 + 1 + ((t * 2) - 2) + _c2)) + 2) / 2;
        }

        private static float EaseOutBack(float t)
        {
            return 1 + (_c3 * Mathf.Pow(f: t - 1, p: 3)) + (_c1 * Mathf.Pow(f: t - 1, p: 2));
        }
    }
}
