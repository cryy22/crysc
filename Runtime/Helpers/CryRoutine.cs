using System.Collections;
using UnityEngine;

namespace Crysc.Helpers
{
    public class CryRoutine : IEnumerator
    {
        private Coroutine _coroutine;
        private bool _isStopping;

        private CryRoutine(MonoBehaviour behaviour, IEnumerator enumerator)
        {
            Behaviour = behaviour;
            Enumerator = enumerator;
        }

        public bool IsComplete { get; private set; }

        private MonoBehaviour Behaviour { get; }
        private IEnumerator Enumerator { get; }

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
                Behaviour.StopCoroutine(_coroutine);
                _coroutine = null;
            }

            IsComplete = true;
            _isStopping = false;
        }

        private void Run() { _coroutine = Behaviour.StartCoroutine(this); }

        public bool MoveNext()
        {
            if (_isStopping)
            {
                HandleStop();
                return false;
            }

            if (Enumerator.MoveNext()) return true;

            IsComplete = true;
            return false;
        }

        public void Reset()
        {
            Enumerator.Reset();
            IsComplete = false;
        }

        public object Current => Enumerator.Current;
    }
}
