using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crysc.Helpers
{
    public static class CoroutineWaiter
    {
        public static IEnumerator RunConcurrently(params Coroutine[] coroutines) { return coroutines.GetEnumerator(); }

        public static IEnumerator RunConcurrently<T>(this IEnumerable<T> behaviours, Func<T, IEnumerator> enumerator)
            where T : MonoBehaviour
        {
            yield return RunConcurrently(
                behaviours.Select(b => b.StartCoroutine(enumerator(b))).ToArray()
            );
        }

        public static IEnumerator RunConcurrently(this MonoBehaviour behaviour, params IEnumerator[] enumerators)
        {
            Coroutine[] coroutines = enumerators.Select(behaviour.StartCoroutine).ToArray();
            return RunConcurrently(coroutines);
        }

        public static IEnumerator RunConcurrently(params ConcurrentCryRoutine[] routines)
        {
            yield return new WaitUntil(() => routines.All(r => r.IsComplete));
        }
    }
}
