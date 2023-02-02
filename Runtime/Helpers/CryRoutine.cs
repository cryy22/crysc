using System.Collections;
using UnityEngine;

namespace Crysc.Helpers
{
    public class CryRoutine : IEnumerator
    {
        private readonly MonoBehaviour _behaviour;
        private readonly IEnumerator _enumerator;
        private Coroutine _coroutine;
        private bool _isStopping;

        private CryRoutine(MonoBehaviour behaviour, IEnumerator enumerator)
        {
            _behaviour = behaviour;
            _enumerator = enumerator;
        }

        public bool IsComplete { get; private set; }

        public static CryRoutine Start(MonoBehaviour behaviour, IEnumerator enumerator)
        {
            CryRoutine routine = new(behaviour: behaviour, enumerator: enumerator);
            routine.Run();
            return routine;
        }

        public void Stop() { _isStopping = true; }

        private void HandleStop()
        {
            if (_coroutine != null)
            {
                _behaviour.StopCoroutine(_coroutine);
                _coroutine = null;
            }

            IsComplete = true;
            _isStopping = false;
        }

        private void Run() { _coroutine = _behaviour.StartCoroutine(this); }

        public bool MoveNext()
        {
            if (_isStopping)
            {
                HandleStop();
                return false;
            }

            if (_enumerator.MoveNext()) return true;

            IsComplete = true;
            _coroutine = null;
            return false;
        }

        public void Reset()
        {
            _enumerator.Reset();
            IsComplete = false;
        }

        public object Current => _enumerator.Current;
    }
}
