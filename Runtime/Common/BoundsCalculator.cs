using UnityEngine;

namespace Crysc.Common
{
    public class BoundsCalculator
    {
        private readonly Collider _collider;
        private readonly Collider2D _collider2D;
        private readonly RectTransform _rectTransform;

        private readonly Canvas _canvas;
        private readonly Camera _camera;

        public BoundsCalculator(Component behaviour)
        {
            _collider = behaviour.GetComponent<Collider>();
            _collider2D = behaviour.GetComponent<Collider2D>();
            _rectTransform = behaviour.GetComponent<RectTransform>();

            if (_collider == null && _collider2D == null && _rectTransform == null)
                throw new MissingComponentException($"No collider or rect transform found on {behaviour.name}");

            if (_collider != null || _collider2D != null) return;
            _canvas = behaviour.GetComponentInParent<Canvas>();
            _camera = Camera.main;
        }

        public Bounds Calculate()
        {
            if (_collider != null) return _collider.bounds;
            if (_collider2D != null) return _collider2D.bounds;

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
    }
}
