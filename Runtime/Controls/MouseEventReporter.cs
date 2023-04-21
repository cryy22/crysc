using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crysc.Controls
{
    public class MouseEventReporter : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] private Component SenderOverrideInput;

        public bool IsHovered { get; private set; }
        private object Sender => SenderOverrideInput ? SenderOverrideInput : this;

        public event EventHandler Hovered;
        public event EventHandler Unhovered;
        public event EventHandler Clicked;

        private void OnDisable()
        {
            if (IsHovered == false) return;

            IsHovered = false;
            Unhovered?.Invoke(sender: Sender, e: EventArgs.Empty);
        }

        public void OnPointerDown(PointerEventData _) { Clicked?.Invoke(sender: Sender, e: EventArgs.Empty); }

        public void OnPointerEnter(PointerEventData _)
        {
            IsHovered = true;
            Hovered?.Invoke(sender: Sender, e: EventArgs.Empty);
        }

        public void OnPointerExit(PointerEventData _)
        {
            IsHovered = false;
            Unhovered?.Invoke(sender: Sender, e: EventArgs.Empty);
        }
    }
}
