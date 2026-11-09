#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

#endregion

namespace Crysc.Helpers
{
    public static class CoroutineWaiter
    {
        private const float _initialDelay = 1.5f;
        private const float _perInstanceDelayPct = 0.9f;

        public static async Awaitable RunExponentiallyAsync<T>(
            this IEnumerable<T> arguments,
            Action<T> action,
            CancellationToken cancellationToken,
            float initialDelay = _initialDelay,
            float perInstanceDelayPct = _perInstanceDelayPct
        )
            where T : MonoBehaviour
        {
            float delay = initialDelay;
            try
            {
                foreach (T argument in arguments)
                {
                    action(argument);
                    await Awaitable.WaitForSecondsAsync(seconds: delay, cancellationToken: cancellationToken);
                    delay *= perInstanceDelayPct;
                }
            }
            catch (OperationCanceledException)
            { }
        }

        public static IEnumerator RunExponentially<T>(
            this List<T> behaviours,
            Func<T, IEnumerator> enumerator,
            float initialDelay = _initialDelay,
            float perInstanceDelayPct = _perInstanceDelayPct
        )
            where T : MonoBehaviour
        {
            List<Coroutine> routines = ListPool<Coroutine>.Get();
            try
            {
                float delay = initialDelay;
                foreach (T behaviour in behaviours)
                {
                    routines.Add(behaviour.StartCoroutine(enumerator(behaviour)));

                    yield return new WaitForSeconds(delay);
                    delay *= perInstanceDelayPct;
                }

                yield return RunConcurrently(routines);
            }
            finally
            {
                ListPool<Coroutine>.Release(routines);
            }
        }

        public static IEnumerator RunConcurrently(IEnumerable<Coroutine> coroutines, int count = int.MaxValue)
        {
            var i = 0;
            foreach (Coroutine coroutine in coroutines)
            {
                if (i >= count)
                    yield break;

                yield return coroutine;
                i++;
            }
        }

        public static IEnumerator RunConcurrently<T>(this IEnumerable<T> behaviours, Func<T, IEnumerator> enumerator)
            where T : MonoBehaviour
        {
            List<Coroutine> routines = ListPool<Coroutine>.Get();
            try
            {
                foreach (T behaviour in behaviours)
                    routines.Add(behaviour.StartCoroutine(enumerator(behaviour)));

                yield return RunConcurrently(routines);
            }
            finally
            {
                ListPool<Coroutine>.Release(routines);
            }
        }

        public static IEnumerator RunConcurrently(this MonoBehaviour behaviour, params IEnumerator[] enumerators)
        {
            List<Coroutine> routines = ListPool<Coroutine>.Get();
            try
            {
                foreach (IEnumerator enumerator in enumerators)
                    routines.Add(behaviour.StartCoroutine(enumerator));

                yield return RunConcurrently(routines);
            }
            finally
            {
                ListPool<Coroutine>.Release(routines);
            }
        }
    }
}
