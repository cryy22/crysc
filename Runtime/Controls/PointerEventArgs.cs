using System;
using UnityEngine;

namespace Crysc.Controls
{
    public class PointerEventArgs : EventArgs
    {
        public Vector2 ScreenPosition { get; }
        public PointerEventArgs(Vector2 screenPosition) { ScreenPosition = screenPosition; }
    }
}
