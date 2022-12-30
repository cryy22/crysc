using UnityEngine;

namespace Crysc.Controls
{
    public class DraggableEventArgs<T> where T : Component
    {
        public DraggableEventArgs(T target) { Target = target; }
        public T Target { get; }
    }
}
