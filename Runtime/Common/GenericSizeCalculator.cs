using UnityEngine;

namespace Crysc.Common
{
    public class GenericSizeCalculator
    {
        private readonly Collider _collider;
        private readonly Collider2D _collider2D;
        private readonly RectTransform _rectTransform;

        private readonly Canvas _canvas;
        private Camera _camera;

        private Camera Camera
        {
            get
            {
                _camera ??= Camera.main;
                return _camera;
            }
        }

        public GenericSizeCalculator(Component behaviour)
        {
            _collider = behaviour.GetComponent<Collider>();
            _collider2D = behaviour.GetComponent<Collider2D>();
            _rectTransform = behaviour.GetComponent<RectTransform>();

            if (_collider == null && _collider2D == null && _rectTransform == null)
                throw new MissingComponentException($"No collider or rect transform found on {behaviour.name}");

            if (_collider != null || _collider2D != null) return;
            _canvas = behaviour.GetComponentInParent<Canvas>();
        }

        public GenericSize Calculate()
        {
            if (_collider != null)
                return new GenericSize(
                    bounds: _collider.bounds,
                    screenDimensions: ScreenDimensionsFromBounds(_collider.bounds)
                );
            if (_collider2D != null)
                return new GenericSize(
                    bounds: _collider2D.bounds,
                    screenDimensions: ScreenDimensionsFromBounds(_collider2D.bounds)
                );

            var rectCorners = new Vector3[4];
            _rectTransform.GetWorldCorners(rectCorners);

            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;

            foreach (Vector3 corner in rectCorners)
            {
                Vector3 worldPoint = _canvas.renderMode == RenderMode.WorldSpace
                    ? corner
                    : Camera.ScreenToWorldPoint(corner);
                min = Vector3.Min(lhs: min, rhs: worldPoint);
                max = Vector3.Max(lhs: max, rhs: worldPoint);
            }

            Bounds bounds = new();
            bounds.SetMinMax(min: min, max: max);
            return new GenericSize(
                bounds: bounds,
                screenDimensions: ScreenDimensionsFromBounds(bounds)
            );
        }

        private Vector2 ScreenDimensionsFromBounds(Bounds bounds)
        {
            if (Camera == null) return Vector2.zero;

            Vector2 min = Camera.WorldToScreenPoint(position: bounds.min);
            Vector2 max = Camera.WorldToScreenPoint(position: bounds.max);
            return max - min;
        }
    }
}
