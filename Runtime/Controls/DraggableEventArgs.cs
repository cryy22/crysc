using System;
using UnityEngine;

namespace Crysc.Controls
{
    public class DraggableEventArgs<T> : EventArgs where T : Component
    {
        public DraggableEventArgs(T target, Vector2 screenPosition)
        {
            Target = target;
            ScreenPosition = screenPosition;
        }

        public T Target { get; }
        public Vector2 ScreenPosition { get; }
    }
}
