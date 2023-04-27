using System;
using Crysc.Common;

namespace Crysc.UI.Tooltips
{
    public interface ITooltipTargetProvider
    {
        public bool IsHovered { get; }
        public bool IgnoreRaycastBlocking { get; }

        public object[] GetTooltipContent();
        public Dimensions GetSize();

        public event EventHandler Hovered;
        public event EventHandler Unhovered;
    }
}
