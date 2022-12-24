using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crysc.UI
{
    public interface IArrangement<in T> where T : IArrangementElement
    {
        public bool IsCentered { get; set; }
        public bool IsInverted { get; set; }
        public Vector2 MaxSize { get; set; }
        public Vector2 PreferredOverhangRatio { get; set; }

        public void SetElements(IEnumerable<T> elements);
        public void Rearrange();
        public IEnumerator AnimateRearrange(float duration);
    }
}
