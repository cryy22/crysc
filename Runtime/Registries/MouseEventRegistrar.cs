using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Crysc.Registries
{
    public abstract class MouseEventRegistrar<T> : Registrar<T>, IMouseEventRegistrar<T>,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
        where T : Object
    {
        private Collider _collider;
        private Collider2D _collider2D;
        private RectTransform _rectTransform;
        private Canvas _canvas;
        private Camera _camera;

        protected override void Awake()
        {
            base.Awake();

            _collider = GetComponent<Collider>();
            _collider2D = GetComponent<Collider2D>();
            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform != null)
            {
                _canvas = GetComponentInParent<Canvas>();
                _camera = Camera.main;
            }
        }

        private void OnDisable() { Unhovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }

        private Bounds GetBounds()
        {
            if (_collider != null) return _collider.bounds;
            if (_collider2D != null) return _collider2D.bounds;
            if (_rectTransform != null)
            {
                var rectCorners = new Vector3[4];
                _rectTransform.GetWorldCorners(rectCorners);

                Vector3 min = Vector3.positiveInfinity;
                Vector3 max = Vector3.negativeInfinity;

                foreach (Vector3 corner in rectCorners)
                {
                    Vector3 worldPoint = _canvas.renderMode == RenderMode.WorldSpace
                        ? corner
                        : _camera.ScreenToWorldPoint(corner);
                    min = Vector3.Min(lhs: min, rhs: worldPoint);
                    max = Vector3.Max(lhs: max, rhs: worldPoint);
                }

                Bounds bounds = new();
                bounds.SetMinMax(min: min, max: max);
                return bounds;
            }

            throw new MissingComponentException($"No collider or rect transform found on {gameObject.name}");
        }

        // IMouseEventRegistrar
        public event EventHandler<RegistryEventArgs<T>> Hovered;
        public event EventHandler<RegistryEventArgs<T>> Unhovered;
        public event EventHandler<RegistryEventArgs<T>> Clicked;
        public Bounds Bounds => GetBounds();

        // IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
        public void OnPointerDown(PointerEventData _) { Clicked?.Invoke(sender: Registrant, e: BuildEventArgs()); }
        public void OnPointerEnter(PointerEventData _) { Hovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }
        public void OnPointerExit(PointerEventData _) { Unhovered?.Invoke(sender: Registrant, e: BuildEventArgs()); }
    }
}
