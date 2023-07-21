using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    public struct ElementPlacement
    {
        public IArrangementElement Element { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public ElementPlacement(IArrangementElement element, Vector3 position, Quaternion rotation)
        {
            Element = element;
            Position = position;
            Rotation = rotation;
        }
    }
}
