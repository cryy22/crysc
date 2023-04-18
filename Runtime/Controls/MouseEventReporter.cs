using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crysc.Controls
{
    public class MouseEventReporter : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public bool IsHovered { get; private set; }

        public event EventHandler Hovered;
        public event EventHandler Unhovered;
        public event EventHandler Clicked;

        private void OnDisable()
        {
            if (IsHovered == false) return;

            IsHovered = false;
            Unhovered?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void OnPointerDown(PointerEventData _) { Clicked?.Invoke(sender: this, e: EventArgs.Empty); }

        public void OnPointerEnter(PointerEventData _)
        {
            IsHovered = true;
            Hovered?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void OnPointerExit(PointerEventData _)
        {
            IsHovered = false;
            Unhovered?.Invoke(sender: this, e: EventArgs.Empty);
        }
    }
}
