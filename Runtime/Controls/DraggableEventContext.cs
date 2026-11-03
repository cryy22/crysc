#region

using UnityEngine;

#endregion

namespace Crysc.Controls
{
    public readonly struct DraggableEventContext<T> where T : Component
    {
        public T Target { get; }
        public Vector2 ScreenPosition { get; }

        public DraggableEventContext(T target, Vector2 screenPosition)
        {
            Target = target;
            ScreenPosition = screenPosition;
        }
    }
}
