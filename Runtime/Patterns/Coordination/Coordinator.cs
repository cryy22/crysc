using System;
using System.Collections;
using UnityEngine;

namespace Crysc.Patterns.Coordination
{
    public abstract class Coordinator<TEvent, TState> : MonoBehaviour
        where TEvent : Enum
        where TState : class
    {
        public event EventHandler<EventArgs> Announcement;
        private readonly WaitGroup _waitGroup = new();

        protected virtual IEnumerator Coordinate(TEvent eventEnum, TState state)
        {
            Announcement?.Invoke(
                sender: this,
                e: new EventArgs(
                    eventEnum: eventEnum,
                    state: state,
                    waitGroup: _waitGroup
                )
            );
            yield return _waitGroup.Wait();
        }

        public class EventArgs : CoordinationEventArgs<TEvent, TState>
        {
            public EventArgs(
                TEvent eventEnum,
                TState state,
                WaitGroup waitGroup
            ) : base(
                eventEnum: eventEnum,
                state: state,
                waitGroup: waitGroup
            )
            { }
        }
    }
}
