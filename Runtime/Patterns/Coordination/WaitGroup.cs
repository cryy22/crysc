#region

using System;
using System.Collections;
using UnityEngine;

#endregion

namespace Crysc.Patterns.Coordination
{
    public class WaitGroup
    {
        public int Count { get; private set; }
        private readonly WaitUntil _waitUntilEmpty;

        public WaitGroup()
        {
            _waitUntilEmpty = new WaitUntil(IsEmpty);
        }

        public void Join()
        {
            Count++;
        }

        public void Leave()
        {
            Count = Math.Max(val1: Count - 1, val2: 0);
        }

        public IEnumerator Wait()
        {
            yield return null;
            yield return _waitUntilEmpty;
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }
    }
}
