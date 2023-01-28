using UnityEngine;

namespace Crysc.Presentation
{
    public interface IArrangementElement
    {
        public Transform Transform { get; }
        public Vector2 SizeMultiplier => Vector2.one;
        public Vector2 Pivot => Vector2.one * 0.5f;
        public Vector3 ArrangementOffset => Vector3.zero;
    }
}
