using System;
using System.Collections;
using UnityEngine;

namespace Crysc.Patterns.Coordination
{
    public abstract class Coordinator<TEvent, TState> : MonoBehaviour
        where TEvent : Enum
        where TState : class
    {
        public event EventHandler<CoordinationEventArgs<TEvent, TState>> Announcement;
        private readonly WaitGroup _waitGroup = new();

        protected IEnumerator AnnounceAndWait(TEvent eventEnum, TState state)
        {
            Announcement?.Invoke(
                sender: this,
                e: new CoordinationEventArgs<TEvent, TState>(
                    eventEnum: eventEnum,
                    state: state,
                    waitGroup: _waitGroup
                )
            );
            yield return _waitGroup.Wait();
        }
    }
}
