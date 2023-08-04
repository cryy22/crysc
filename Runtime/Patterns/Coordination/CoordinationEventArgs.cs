using System;

namespace Crysc.Patterns.Coordination
{
    public class CoordinationEventArgs<TEvent, TState> : EventArgs
        where TEvent : Enum
        where TState : class
    {
        public WaitGroup WaitGroup { get; }
        public TEvent Event { get; }
        public TState State { get; }

        public CoordinationEventArgs(TEvent eventEnum, TState state, WaitGroup waitGroup)
        {
            Event = eventEnum;
            State = state;
            WaitGroup = waitGroup;
        }
    }
}
