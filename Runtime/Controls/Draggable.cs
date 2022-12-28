using System;
using UnityEngine;

namespace Crysc.Controls
{
    [RequireComponent(typeof(Collider2D))]
    public class Draggable : MonoBehaviour
    {
        private Collider2D _collider;
        private Camera _camera;
        private Vector2 _initialClickOffset;

        public event EventHandler Began;
        public event EventHandler Ended;

        private void Awake() { _camera = Camera.main; }

        private void OnMouseDown()
        {
            Vector2 cursor = _camera.ScreenToWorldPoint(Input.mousePosition);
            _initialClickOffset = (Vector2) transform.position - cursor;

            Began?.Invoke(sender: this, e: EventArgs.Empty);
        }

        private void OnMouseDrag()
        {
            Transform tf = transform;
            Vector2 cursor = (Vector2) _camera.ScreenToWorldPoint(Input.mousePosition) + _initialClickOffset;

            tf.position = new Vector3(x: cursor.x, y: cursor.y, z: tf.position.z);
        }

        private void OnMouseUp() { Ended?.Invoke(sender: this, e: EventArgs.Empty); }
    }
}
