using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crysc.Controls
{
    public class PointerEventReporter : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public event EventHandler<PointerEventArgs> Dragged;
        public event EventHandler<PointerEventArgs> Hovered;
        public event EventHandler<PointerEventArgs> Pressed;
        public event EventHandler<PointerEventArgs> Unhovered;
        public event EventHandler<PointerEventArgs> Unpressed;

        [SerializeField] private Component SenderOverrideInput;

        public bool IsHovered { get; private set; }
        private object Sender => SenderOverrideInput ? SenderOverrideInput : this;

        private Vector2 _latestScreenPosition;

        private void OnDisable()
        {
            if (IsHovered == false) return;

            IsHovered = false;

            Unhovered?.Invoke(sender: Sender, e: CreatePointerEventArgs());
        }

        public void OnPointerEnter(PointerEventData data)
        {
            IsHovered = true;
            _latestScreenPosition = data.position;

            Hovered?.Invoke(sender: Sender, e: CreatePointerEventArgs());
        }

        public void OnPointerExit(PointerEventData data)
        {
            IsHovered = false;
            _latestScreenPosition = data.position;

            Unhovered?.Invoke(sender: Sender, e: CreatePointerEventArgs());
        }

        public void OnPointerDown(PointerEventData data)
        {
            _latestScreenPosition = data.position;
            Pressed?.Invoke(sender: Sender, e: CreatePointerEventArgs());
        }

        public void OnPointerUp(PointerEventData data)
        {
            _latestScreenPosition = data.position;
            Unpressed?.Invoke(sender: Sender, e: CreatePointerEventArgs());
        }

        public void OnDrag(PointerEventData data)
        {
            _latestScreenPosition = data.position;
            Dragged?.Invoke(sender: Sender, e: CreatePointerEventArgs());
        }

        private PointerEventArgs CreatePointerEventArgs() { return new PointerEventArgs(_latestScreenPosition); }
    }
}
