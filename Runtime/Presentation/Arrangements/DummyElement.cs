using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    using IElement = IArrangementElement;

    public class DummyElement : MonoBehaviour, IElement
    {
        public Transform Transform => transform;
        public Vector2 SizeMultiplier { get; set; } = IElement.DefaultSizeMultiplier;
        public Vector2 Pivot { get; set; } = IElement.DefaultPivot;
        public Vector3 ArrangementOffset { get; set; } = IElement.DefaultArrangementOffset;

        public void MimicElement(IElement element)
        {
            SizeMultiplier = element.SizeMultiplier;
            Pivot = element.Pivot;
            ArrangementOffset = element.ArrangementOffset;
        }

        public void RestoreDefaults()
        {
            SizeMultiplier = IElement.DefaultSizeMultiplier;
            Pivot = IElement.DefaultPivot;
            ArrangementOffset = IElement.DefaultArrangementOffset;
        }
    }
}
