using System;
using UnityEngine;

namespace Crysc.Controls
{
    [RequireComponent(typeof(Collider2D))]
    public class Draggable<T> : MonoBehaviour where T : Component
    {
        private Collider2D _collider;
        private Camera _camera;
        private Vector2 _initialClickOffset;
        private T _target;

        public event EventHandler<DraggableEventArgs<T>> Began;
        public event EventHandler<DraggableEventArgs<T>> Ended;

        [field: SerializeField] public bool IsActive { get; set; }

        private void Awake()
        {
            _camera = Camera.main;
            _target = GetComponent<T>();
        }

        private void OnMouseDown()
        {
            if (!IsActive) return;

            Vector2 cursor = _camera.ScreenToWorldPoint(Input.mousePosition);
            _initialClickOffset = (Vector2) transform.position - cursor;

            Began?.Invoke(sender: this, e: new DraggableEventArgs<T>(_target));
        }

        private void OnMouseDrag()
        {
            if (!IsActive) return;

            Vector2 cursor = (Vector2) _camera.ScreenToWorldPoint(Input.mousePosition) + _initialClickOffset;

            Transform draggableTransform = transform;
            draggableTransform.position = new Vector3(x: cursor.x, y: cursor.y, z: draggableTransform.position.z);
        }

        private void OnMouseUp()
        {
            if (!IsActive) return;

            Ended?.Invoke(sender: this, e: new DraggableEventArgs<T>(_target));
        }
    }

    public class Draggable : Draggable<Component>
    { }
}
