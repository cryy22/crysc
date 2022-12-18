using UnityEngine;

namespace Crysc.UI
{
    public interface IArrangementElement
    {
        public Transform Transform { get; }
        public Bounds Bounds { get; }
    }
}
