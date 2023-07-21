using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crysc.Presentation.Arrangements
{
    public interface IArrangement<in T>
    {
        public Arrangement.Alignment HorizontalAlignment { get; set; }
        public bool IsInverted { get; set; }
        public Vector2 MaxSize { get; set; }
        public Vector2 PreferredSpacingRatio { get; set; }

        public void SetElements(IEnumerable<T> elements);
        public void Rearrange();
        public IEnumerator AnimateRearrange(float duration);
    }

    public interface IArrangement : IArrangement<IArrangementElement>
    { }
}
