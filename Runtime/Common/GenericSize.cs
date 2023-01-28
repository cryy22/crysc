using UnityEngine;

namespace Crysc.Common
{
    public struct GenericSize
    {
        public Bounds Bounds { get; }
        public Vector2 ScreenDimensions { get; }

        public GenericSize(Bounds bounds, Vector2 screenDimensions)
        {
            Bounds = bounds;
            ScreenDimensions = screenDimensions;
        }
    }
}
