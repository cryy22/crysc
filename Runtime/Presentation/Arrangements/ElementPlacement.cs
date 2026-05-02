using JetBrains.Annotations;
using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    public readonly struct ElementPlacement
    {
        public IArrangementElement Element { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public Vector3 Scale { get; }

        public ElementPlacement(IArrangementElement element, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Element = element;
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public ElementPlacement Copy(
            [CanBeNull] IArrangementElement element = null,
            Vector3? position = null,
            Quaternion? rotation = null,
            Vector3? scale = null
        )
        {
            return new ElementPlacement(
                element ?? Element,
                position ?? Position,
                rotation ?? Rotation,
                scale ?? Scale
            );
        }
    }
}
