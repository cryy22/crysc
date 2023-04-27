using UnityEngine;

namespace Crysc.Helpers
{
    public static class BoundsOperator
    {
        public static Bounds Make2D(Bounds bounds)
        {
            return new Bounds(
                center: new Vector3(
                    x: bounds.center.x,
                    y: bounds.center.y,
                    z: 0
                ),
                size: new Vector3(
                    x: bounds.size.x,
                    y: bounds.size.y,
                    z: float.PositiveInfinity
                )
            );
        }
    }
}
