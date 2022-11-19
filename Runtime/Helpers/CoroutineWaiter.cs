using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crysc.Helpers
{
    public static class CoroutineWaiter
    {
        public static IEnumerator RunConcurrently<T>(
            IEnumerable<T> behaviours,
            Func<T, IEnumerator> enumerator
        ) where T : MonoBehaviour
        {
            yield return RunConcurrently(
                behaviours
                    .Select(b => b.StartCoroutine(enumerator(b)))
                    .ToArray()
            );
        }

        public static IEnumerator RunConcurrently(params Coroutine[] coroutines) { return coroutines.GetEnumerator(); }
    }
}
