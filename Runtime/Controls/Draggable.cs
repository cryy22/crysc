using System;
using UnityEngine;

namespace Crysc.Controls
{
    [RequireComponent(typeof(PointerEventReporter))]
    public class Draggable<T> : MonoBehaviour where T : Component
    {
        public event EventHandler<DraggableEventArgs<T>> Began;
        public event EventHandler<DraggableEventArgs<T>> Ended;
        public event EventHandler<DraggableEventArgs<T>> Moved;

        private const float _zAxis = -5f;

        [field: SerializeField] public bool IsActive { get; set; }

        private PointerEventReporter _pointerEventReporter;
        private Camera _camera;
        private Vector2 _initialClickOffset;
        private T _target;

        protected virtual void Awake()
        {
            _pointerEventReporter = GetComponent<PointerEventReporter>();
            _camera = Camera.main;
            _target = GetComponentInParent<T>();
        }

        private void OnEnable()
        {
            _pointerEventReporter.Pressed += OnPointerDown;
            _pointerEventReporter.Dragged += OnPointerDragged;
            _pointerEventReporter.Unpressed += OnPointerUp;
        }

        private void OnDisable()
        {
            _pointerEventReporter.Pressed -= OnPointerDown;
            _pointerEventReporter.Dragged -= OnPointerDragged;
            _pointerEventReporter.Unpressed -= OnPointerUp;
        }

        private void OnPointerDown(object sender, PointerEventArgs e)
        {
            if (!IsActive) return;

            Vector2 cursor = _camera.ScreenToWorldPoint(e.ScreenPosition);
            _initialClickOffset = (Vector2) transform.position - cursor;

            Began?.Invoke(
                sender: this,
                e: new DraggableEventArgs<T>(
                    target: _target,
                    screenPosition: _camera.WorldToScreenPoint(cursor + _initialClickOffset)
                )
            );
        }

        private void OnPointerUp(object sender, PointerEventArgs e)
        {
            if (!IsActive) return;

            Ended?.Invoke(
                sender: this,
                e: new DraggableEventArgs<T>(
                    target: _target,
                    screenPosition: e.ScreenPosition + (Vector2) _camera.WorldToScreenPoint(_initialClickOffset)
                )
            );
        }

        private void OnPointerDragged(object sender, PointerEventArgs e)
        {
            if (!IsActive) return;

            Vector2 cursor = (Vector2) _camera.ScreenToWorldPoint(e.ScreenPosition) + _initialClickOffset;

            Transform targetTransform = _target.transform;
            targetTransform.position = new Vector3(x: cursor.x, y: cursor.y, z: _zAxis);

            Moved?.Invoke(
                sender: this,
                e: new DraggableEventArgs<T>(
                    target: _target,
                    screenPosition: _camera.WorldToScreenPoint(cursor)
                )
            );
        }
    }

    public class Draggable : Draggable<Component>
    { }
}
