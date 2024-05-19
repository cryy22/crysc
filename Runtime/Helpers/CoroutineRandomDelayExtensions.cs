using System.Collections;
using UnityEngine;

namespace Crysc.Helpers
{
    public static class CoroutineRandomDelayExtensions
    {
        public static IEnumerator WithRandomDelay(this IEnumerator enumerator, float maxDelay)
        {
            yield return new WaitForSeconds(Random.Range(minInclusive: 0f, maxInclusive: maxDelay));
            yield return enumerator;
        }
    }
}
