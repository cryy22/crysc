using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crysc.Helpers
{
    public static class CoroutineWaiter
    {
        private const float _initialDelay = 1.5f;
        private const float _perInstanceDelayPct = 0.9f;

        public static IEnumerator RunExponentially(
            this MonoBehaviour behaviour,
            float initialDelay = _initialDelay,
            float perInstanceDelayPct = _perInstanceDelayPct,
            params IEnumerator[] enumerators
        )
        {
            List<Coroutine> routines = new();
            float delay = initialDelay;
            foreach (IEnumerator enumerator in enumerators)
            {
                routines.Add(behaviour.StartCoroutine(enumerator));
                yield return new WaitForSeconds(delay);
                delay *= perInstanceDelayPct;
            }

            yield return RunConcurrently(routines.ToArray());
        }

        public static IEnumerator RunExponentially<T>(
            this IEnumerable<T> behaviours,
            Func<T, IEnumerator> enumerator,
            float initialDelay = _initialDelay,
            float perInstanceDelayPct = _perInstanceDelayPct
        )
            where T : MonoBehaviour
        {
            List<Coroutine> routines = new();
            float delay = initialDelay;
            foreach (T behaviour in behaviours)
            {
                routines.Add(behaviour.StartCoroutine(enumerator(behaviour)));
                yield return new WaitForSeconds(delay);
                delay *= perInstanceDelayPct;
            }

            yield return RunConcurrently(routines.ToArray());
        }

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
