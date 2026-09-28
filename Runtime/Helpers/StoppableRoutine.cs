#region

using System.Collections;
using UnityEngine;

#endregion

namespace Crysc.Helpers
{
    public class StoppableRoutine : IEnumerator
    {
        public bool IsComplete { get; private set; }
        public object Current => _child ?? _enumerator.Current;
        private readonly IEnumerator _enumerator;
        private StoppableRoutine _child;

        public StoppableRoutine(IEnumerator enumerator)
        {
            _enumerator = enumerator;
        }

        public StoppableRoutine(IEnumerator enumerator, MonoBehaviour behaviour)
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
                if (_enumerator.Current is IEnumerator subEnumerator) _child = new StoppableRoutine(subEnumerator);
                return true;
            }

            IsComplete = true;
            return false;
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        public IEnumerator WaitForCompletion()
        {
            while (!IsComplete) yield return null;
        }

        public static StoppableRoutine Start(MonoBehaviour behaviour, IEnumerator enumerator)
        {
            return new StoppableRoutine(behaviour: behaviour, enumerator: enumerator);
        }

        public void Stop()
        {
            IsComplete = true;
            _child?.Stop();
        }
    }
}
