using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    public interface IArrangementElement
    {
        public static readonly Vector2 DefaultSizeMultiplier = Vector2.one;
        public static readonly Vector2 DefaultPivot = Vector2.one * 0.5f;
        public static readonly Vector3 DefaultArrangementOffset = Vector3.zero;

        public Transform Transform { get; }
        public Vector2 SizeMultiplier => DefaultSizeMultiplier;
        public Vector2 Pivot => DefaultPivot;
        public Vector3 ArrangementOffset => DefaultArrangementOffset;
    }
}
