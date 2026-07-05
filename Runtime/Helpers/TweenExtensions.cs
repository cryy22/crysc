using System.Collections;
using PrimeTween;

namespace Crysc.Helpers
{
    public static class TweenExtensions
    {
        // Unlike ToYieldInstruction, stops the tween when the wrapping coroutine is
        // stopped (StopCoroutine runs iterator finally blocks via Dispose).
        public static IEnumerator ToStoppableYield(this Tween tween)
        {
            try
            {
                while (tween.isAlive) yield return null;
            }
            finally
            {
                if (tween.isAlive) tween.Stop();
            }
        }
    }
}
