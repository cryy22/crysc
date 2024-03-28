using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    public abstract class Easings
    {
        public enum Enum
        {
            Linear,
            EaseOutElastic,
        }

        public static float Ease(float t, Enum easing)
        {
            return easing switch
            {
                Enum.EaseOutElastic => EaseOutElastic(t),
                Enum.Linear         => Linear(t),
                _                   => Linear(t),
            };
        }

        private static float Linear(float t) { return t; }

        private static float EaseOutElastic(float t)
        {
            const float c4 = (2 * Mathf.PI) / 3;

            return t switch
            {
                0 => 0,
                1 => 1,
                _ => (Mathf.Pow(f: 2, p: -10 * t) * Mathf.Sin((float) (((t * 10) - 0.75) * c4))) + 1,
            };
        }
    }
}
