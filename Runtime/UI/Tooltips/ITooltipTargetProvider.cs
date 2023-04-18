using System;
using UnityEngine;

namespace Crysc.UI.Tooltips
{
    public interface ITooltipTargetProvider
    {
        public bool IsHovered { get; }

        public event EventHandler Hovered;
        public event EventHandler Unhovered;

        public TooltipContent[] GetTooltipContent();
        public Bounds GetBounds();
    }
}
