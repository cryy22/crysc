using System.Collections;
using UnityEngine;

namespace Crysc.Patterns.Coordination
{
    public class WaitGroup
    {
        public bool IsEmpty => _count <= 0;

        private int _count;
        private readonly WaitUntil _waitUntilEmpty;

        public WaitGroup() { _waitUntilEmpty = new WaitUntil(() => IsEmpty); }

        public void Join() { _count++; }
        public void Leave() { _count--; }

        public IEnumerator Wait() { yield return _waitUntilEmpty; }
    }
}
