#region

using System;
using System.Collections;
using UnityEngine;

#endregion

namespace Crysc.Patterns.Coordination
{
    public abstract class Coordinator<TEvent, TState> : MonoBehaviour
        where TEvent : Enum
        where TState : class
    {
        public event Action<EventContext> Announcement;

        protected virtual IEnumerator Coordinate(TEvent eventEnum, TState state)
        {
            WaitGroup waitGroup = new();
            waitGroup.Join();
            Announcement?.Invoke(new EventContext(eventEnum: eventEnum, state: state, waitGroup: waitGroup));

            waitGroup.Leave();
            yield return null;

            while (waitGroup.IsEmpty() == false)
            {
                yield return waitGroup.Wait();
                yield return null;
            }
        }

        protected virtual void CoordinateWithoutWaiting(TEvent eventEnum, TState state)
        {
            Announcement?.Invoke(new EventContext(eventEnum: eventEnum, state: state, waitGroup: null));
        }

        public readonly struct EventContext
        {
            public WaitGroup WaitGroup { get; }
            public TEvent Event { get; }
            public TState State { get; }

            public EventContext(TEvent eventEnum, TState state, WaitGroup waitGroup)
            {
                Event = eventEnum;
                State = state;
                WaitGroup = waitGroup;
            }
        }
    }
}
