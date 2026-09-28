#region

using System.Collections;
using System.Linq;
using UnityEngine;

#endregion

namespace Crysc.Helpers
{
    public class ConcurrentStoppableRoutine : IEnumerator
    {
        public bool IsComplete { get; private set; }
        public object Current => null;

        private readonly StoppableRoutine[] _routines;

        public ConcurrentStoppableRoutine(params IEnumerator[] enumerators)
        {
            _routines = enumerators.Select(e => new StoppableRoutine(e)).ToArray();
        }

        public ConcurrentStoppableRoutine(MonoBehaviour behaviour, params IEnumerator[] enumerators)
        {
            _routines = enumerators.Select(e => new StoppableRoutine(e)).ToArray();
            behaviour.StartCoroutine(this);
        }

        public bool MoveNext()
        {
            if (IsComplete) return false;

            if (_routines.Aggregate(seed: false, func: (inProgress, routine) => routine.MoveNext() || inProgress))
                return true;

            IsComplete = true;
            return false;
        }

        public void Reset()
        {
            foreach (StoppableRoutine routine in _routines) routine.Reset();
        }

        public void Stop()
        {
            IsComplete = true;
            foreach (StoppableRoutine routine in _routines) routine.Stop();
        }
    }
}
