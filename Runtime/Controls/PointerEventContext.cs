#region

using UnityEngine;

#endregion

namespace Crysc.Controls
{
    public readonly struct PointerEventContext
    {
        public Component Sender { get; }
        public Vector2 ScreenPosition { get; }

        public PointerEventContext(Component sender, Vector2 screenPosition)
        {
            Sender = sender;
            ScreenPosition = screenPosition;
        }
    }
}
