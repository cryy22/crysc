using System;
using UnityEngine;

namespace Crysc.Controls
{
    public class DraggableEventArgs<T> : EventArgs where T : Component
    {
        public DraggableEventArgs(T target) { Target = target; }
        public T Target { get; }
    }
}
