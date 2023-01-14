using System;

namespace Crysc.EventProps
{
    public class ToggleEventArgs : EventArgs
    {
        public ToggleEventArgs(bool current, bool previous)
        {
            Current = current;
            Previous = previous;
        }

        public bool Current { get; }
        public bool Previous { get; }
    }
}
