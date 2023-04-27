using Crysc.Helpers;
using UnityEngine;

namespace Crysc.Common
{
    public readonly struct Dimensions
    {
        public Bounds WorldBounds { get; }
        public Bounds ScreenBounds { get; }

        public Dimensions(Bounds worldBounds, Bounds screenBounds)
        {
            WorldBounds = BoundsOperator.Make2D(worldBounds);
            ScreenBounds = BoundsOperator.Make2D(screenBounds);
        }

        public bool IsScreenPointWithin(Vector2 screenPoint) { return ScreenBounds.Contains(screenPoint); }
        public bool IsWorldPointWithin(Vector2 worldPoint) { return WorldBounds.Contains(worldPoint); }
    }
}
