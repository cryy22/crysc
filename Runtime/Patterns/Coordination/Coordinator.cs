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

        protected virtual IEnumerator Coordinate(TEvent eventEnum, TState state)
        {
            WaitGroup waitGroup = new();
            waitGroup.Join();
            Announcement?.Invoke(
                sender: this,
                e: new EventArgs(eventEnum: eventEnum, state: state, waitGroup: waitGroup)
            );

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
            Announcement?.Invoke(
                sender: this,
                e: new EventArgs(eventEnum: eventEnum, state: state, waitGroup: null)
            );
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
