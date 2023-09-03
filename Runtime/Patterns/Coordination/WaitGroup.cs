using System;
using System.Collections;
using UnityEngine;

namespace Crysc.Patterns.Coordination
{
    public class WaitGroup
    {
        private int _count;
        private readonly WaitUntil _waitUntilEmpty;

        public WaitGroup() { _waitUntilEmpty = new WaitUntil(IsEmpty); }

        public void Join() { _count++; }
        public void Leave() { _count = Math.Max(val1: _count - 1, val2: 0); }

        public IEnumerator Wait() { yield return _waitUntilEmpty; }
        public bool IsEmpty() { return _count == 0; }
    }
}
