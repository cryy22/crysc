#region

using System;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Crysc.Controls
{
    public class PointerEventReporter : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        public event Action<PointerEventContext> Hovered;
        public event Action<PointerEventContext> Unhovered;
        public event Action<PointerEventContext> Pressed;
        public event Action<PointerEventContext> Unpressed;
        public event Action<PointerEventContext> Clicked;
        public event Action<PointerEventContext> BegunDrag;
        public event Action<PointerEventContext> Dragged;
        public event Action<PointerEventContext> DragEnded;

        [SerializeField] private Component SenderOverrideInput;

        public bool IsHovered { get; private set; }
        private Component Sender => SenderOverrideInput ? SenderOverrideInput : this;

        private Vector2 _latestScreenPosition;

        private void OnDisable()
        {
            if (IsHovered == false)
                return;

            IsHovered = false;
            Unhovered?.Invoke(CreatePointerEventContext());
        }

        public void OnPointerEnter(PointerEventData data)
        {
            IsHovered = true;
            _latestScreenPosition = data.position;

            Hovered?.Invoke(CreatePointerEventContext());
        }

        public void OnPointerExit(PointerEventData data)
        {
            IsHovered = false;
            _latestScreenPosition = data.position;

            Unhovered?.Invoke(CreatePointerEventContext());
        }

        public void OnPointerDown(PointerEventData data)
        {
            _latestScreenPosition = data.position;
            Pressed?.Invoke(CreatePointerEventContext());
        }

        public void OnPointerUp(PointerEventData data)
        {
            _latestScreenPosition = data.position;
            Unpressed?.Invoke(CreatePointerEventContext());
        }

        public void OnPointerClick(PointerEventData data)
        {
            _latestScreenPosition = data.position;
            Clicked?.Invoke(CreatePointerEventContext());
        }

        public void OnBeginDrag(PointerEventData data)
        {
            _latestScreenPosition = data.position;
            BegunDrag?.Invoke(CreatePointerEventContext());
        }

        public void OnDrag(PointerEventData data)
        {
            _latestScreenPosition = data.position;
            Dragged?.Invoke(CreatePointerEventContext());
        }

        public void OnEndDrag(PointerEventData data)
        {
            _latestScreenPosition = data.position;
            DragEnded?.Invoke(CreatePointerEventContext());
        }

        public void ClearSubscribers()
        {
            Hovered = null;
            Unhovered = null;
            Pressed = null;
            Unpressed = null;
            Clicked = null;
            BegunDrag = null;
            Dragged = null;
            DragEnded = null;
        }

        private PointerEventContext CreatePointerEventContext()
        {
            return new PointerEventContext(sender: Sender, screenPosition: _latestScreenPosition);
        }
    }
}
