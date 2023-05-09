using System.Collections;
using UnityEngine;

namespace Crysc.Helpers
{
    public class CryRoutine : IEnumerator
    {
        public bool IsComplete { get; private set; }
        public object Current => _child ?? _enumerator.Current;
        private readonly IEnumerator _enumerator;
        private CryRoutine _child;

        public CryRoutine(IEnumerator enumerator) { _enumerator = enumerator; }

        public CryRoutine(IEnumerator enumerator, MonoBehaviour behaviour)
        {
            _enumerator = enumerator;
            behaviour.StartCoroutine(this);
        }

        public bool MoveNext()
        {
            if (IsComplete) return false;

            if (_child != null)
            {
                if (_child.MoveNext()) return true;
                _child = null;
            }

            if (_enumerator.MoveNext())
            {
                if (_enumerator.Current is IEnumerator subEnumerator) _child = new CryRoutine(subEnumerator);
                return true;
            }

            IsComplete = true;
            return false;
        }

        public void Reset() { _enumerator.Reset(); }

        public static CryRoutine Start(MonoBehaviour behaviour, IEnumerator enumerator)
        {
            return new CryRoutine(behaviour: behaviour, enumerator: enumerator);
        }

        public void Stop()
        {
            IsComplete = true;
            _child?.Stop();
        }
    }
}
