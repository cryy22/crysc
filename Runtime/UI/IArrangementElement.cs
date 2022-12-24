using UnityEngine;

namespace Crysc.UI
{
    public interface IArrangementElement
    {
        public Transform Transform { get; }
        public Vector2 SizeMultiplier => Vector2.one;
        public Vector2 Pivot => Vector2.one * 0.5f;
    }
}
