#region

using System;
using UnityEngine;

#endregion

namespace Crysc.Controls
{
    [RequireComponent(typeof(PointerEventReporter))]
    public class Draggable<T> : MonoBehaviour where T : Component
    {
        public event Action<DraggableEventContext<T>> Began;
        public event Action<DraggableEventContext<T>> Ended;
        public event Action<DraggableEventContext<T>> Moved;

        private const float _zAxis = -5f;

        [field: SerializeField] public bool IsActive { get; set; }

        private PointerEventReporter _pointerReporter;
        private Camera _camera;
        private Vector2 _initialClickOffset;
        private T _target;
        private bool _isDragging;

        protected virtual void Awake()
        {
            _pointerReporter = GetComponent<PointerEventReporter>();
            _camera = Camera.main;
            _target = GetComponentInParent<T>();
        }

        private void OnEnable()
        {
            _pointerReporter.Pressed += OnPointerDown;
            _pointerReporter.Dragged += OnPointerDragged;
            _pointerReporter.Unpressed += OnPointerUp;
        }

        private void OnDisable()
        {
            _pointerReporter.Pressed -= OnPointerDown;
            _pointerReporter.Dragged -= OnPointerDragged;
            _pointerReporter.Unpressed -= OnPointerUp;
        }

        private void OnPointerDown(PointerEventContext ctx)
        {
            if (!IsActive)
                return;

            _isDragging = true;
            Vector2 cursor = _camera.ScreenToWorldPoint(ctx.ScreenPosition);
            _initialClickOffset = (Vector2) transform.position - cursor;

            Began?.Invoke(
                new DraggableEventContext<T>(
                    target: _target,
                    screenPosition: _camera.WorldToScreenPoint(cursor + _initialClickOffset)
                )
            );
        }

        private void OnPointerUp(PointerEventContext ctx)
        {
            if (!IsActive || !_isDragging)
                return;

            _isDragging = false;
            Ended?.Invoke(
                new DraggableEventContext<T>(
                    target: _target,
                    screenPosition: ctx.ScreenPosition + (Vector2) _camera.WorldToScreenPoint(_initialClickOffset)
                )
            );
        }

        private void OnPointerDragged(PointerEventContext ctx)
        {
            if (!IsActive || !_isDragging)
                return;

            Vector2 cursor = (Vector2) _camera.ScreenToWorldPoint(ctx.ScreenPosition) + _initialClickOffset;

            Transform targetTransform = _target.transform;
            targetTransform.position = new Vector3(x: cursor.x, y: cursor.y, z: _zAxis);

            Moved?.Invoke(
                new DraggableEventContext<T>(
                    target: _target,
                    screenPosition: _camera.WorldToScreenPoint(cursor)
                )
            );
        }
    }

    public class Draggable : Draggable<Component>
    { }
}
