using System.Collections;
using UnityEngine;

namespace Crysc.Helpers
{
    public class CryRoutine : IEnumerator
    {
        private readonly IEnumerator _enumerator;

        public CryRoutine(IEnumerator enumerator, MonoBehaviour behaviour)
        {
            _enumerator = enumerator;
            behaviour.StartCoroutine(this);
        }

        public bool IsComplete { get; private set; }
        public object Current => _enumerator.Current;

        public bool MoveNext()
        {
            if (IsComplete) return false;
            if (Current is IEnumerator subEnumerator && subEnumerator.MoveNext()) return true;
            if (_enumerator.MoveNext()) return true;

            IsComplete = true;
            return false;
        }

        public void Reset() { _enumerator.Reset(); }

        public static CryRoutine Start(MonoBehaviour behaviour, IEnumerator enumerator)
        {
            CryRoutine routine = new(behaviour: behaviour, enumerator: enumerator);
            return routine;
        }

        public void Stop() { IsComplete = true; }
    }
}
