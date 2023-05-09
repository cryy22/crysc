using System.Collections;
using System.Linq;
using UnityEngine;

namespace Crysc.Helpers
{
    public class ConcurrentCryRoutine : IEnumerator
    {
        public bool IsComplete { get; private set; }
        public object Current => null;

        private readonly CryRoutine[] _routines;

        public ConcurrentCryRoutine(params IEnumerator[] enumerators)
        {
            _routines = enumerators.Select(e => new CryRoutine(e)).ToArray();
        }

        public ConcurrentCryRoutine(MonoBehaviour behaviour, params IEnumerator[] enumerators)
        {
            _routines = enumerators.Select(e => new CryRoutine(e)).ToArray();
            behaviour.StartCoroutine(this);
        }

        public bool MoveNext()
        {
            if (IsComplete) return false;

            if (_routines.Aggregate(seed: false, (inProgress, routine) => routine.MoveNext() || inProgress))
                return true;

            IsComplete = true;
            return false;
        }

        public void Reset()
        {
            foreach (CryRoutine routine in _routines) routine.Reset();
        }

        public void Stop()
        {
            IsComplete = true;
            foreach (CryRoutine routine in _routines) routine.Stop();
        }
    }
}
