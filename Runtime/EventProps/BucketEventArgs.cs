using System;

namespace Crysc.EventProps
{
    public class BucketEventArgs<T> : EventArgs
    {
        public BucketEventArgs(T previous, T current, T max)
        {
            Previous = previous;
            Current = current;
            Max = max;
        }

        public T Previous { get; }
        public T Current { get; }
        public T Max { get; }
    }
}
