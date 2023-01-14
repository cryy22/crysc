using System;
using UnityEngine;

namespace Crysc.EventProps
{
    public class Toggle
    {
        public Toggle(bool current = false) { Current = current; }

        public event EventHandler<ToggleEventArgs> Changed;

        [field: SerializeField] public bool Current { get; private set; }

        public void ToggleValue() { SetValue(!Current); }

        public void SetValue(bool value)
        {
            bool previous = Current;
            Current = value;

            Changed?.Invoke(sender: this, e: new ToggleEventArgs(current: Current, previous: previous));
        }
    }
}
