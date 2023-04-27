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

        private Camera Camera => _camera ? _camera : _camera = Camera.main;

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

        public Dimensions Calculate()
        {
            if (_collider != null)
                return new Dimensions(
                    worldBounds: _collider.bounds,
                    screenBounds: ScreenBoundsFromWorldBounds(_collider.bounds)
                );
            if (_collider2D != null)
                return new Dimensions(
                    worldBounds: _collider2D.bounds,
                    screenBounds: ScreenBoundsFromWorldBounds(_collider2D.bounds)
                );

            var rectCorners = new Vector3[4];
            _rectTransform.GetWorldCorners(rectCorners);

            Vector3 worldMin = Vector3.positiveInfinity;
            Vector3 worldMax = Vector3.negativeInfinity;

            foreach (Vector3 corner in rectCorners)
            {
                Vector3 worldPoint = _canvas.renderMode == RenderMode.ScreenSpaceOverlay
                    ? Camera.ScreenToWorldPoint(corner)
                    : corner;
                worldMin = Vector3.Min(lhs: worldMin, rhs: worldPoint);
                worldMax = Vector3.Max(lhs: worldMax, rhs: worldPoint);
            }

            Bounds worldBounds = new();
            worldBounds.SetMinMax(min: worldMin, max: worldMax);
            return new Dimensions(
                worldBounds: worldBounds,
                screenBounds: ScreenBoundsFromWorldBounds(worldBounds)
            );
        }

        private Bounds ScreenBoundsFromWorldBounds(Bounds worldBounds)
        {
            Bounds screenBounds = new();
            screenBounds.SetMinMax(
                min: Camera.WorldToScreenPoint(position: worldBounds.min),
                max: Camera.WorldToScreenPoint(position: worldBounds.max)
            );
            return screenBounds;
        }
    }
}
