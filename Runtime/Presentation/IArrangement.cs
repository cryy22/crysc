using System.Collections;
using System.Collections.Generic;
using Crysc.UI;
using UnityEngine;

namespace Crysc.Presentation
{
    public interface IArrangement<in T> where T : IArrangementElement
    {
        public bool IsCentered { get; set; }
        public bool IsInverted { get; set; }
        public Vector2 MaxSize { get; set; }
        public Vector2 PreferredSpacingRatio { get; set; }

        public void SetElements(IEnumerable<T> elements);
        public void Rearrange();
        public IEnumerator AnimateRearrange(float duration);
    }
}