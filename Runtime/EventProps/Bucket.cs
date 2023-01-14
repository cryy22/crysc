using System;
using UnityEngine;

namespace Crysc.EventProps
{
    [Serializable]
    public class Bucket<T>
    {
        public Bucket() { }
        public Bucket(T value) { Initialize(value); }

        public event EventHandler<BucketEventArgs<T>> Changed;

        [field: SerializeField] public T Max { get; private set; }
        [field: SerializeField] public T Current { get; private set; }

        public virtual void SetValue(T value)
        {
            T previous = Current;

            var eventArgs = new BucketEventArgs<T>(previous: previous, current: Current, max: Max);
            Changed?.Invoke(sender: this, e: eventArgs);
        }

        public void SetMax(T value)
        {
            T previous = Max;
            Max = value;

            var eventArgs = new BucketEventArgs<T>(previous: previous, current: Current, max: Max);
            Changed?.Invoke(sender: this, e: eventArgs);
        }

        public void Fill() { SetValue(value: Max); }

        public void Initialize(T value)
        {
            Changed = null;

            Max = value;
            Current = value;
        }
    }
}
