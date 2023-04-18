using System;
using UnityEngine;

namespace Crysc.UI.Tooltips
{
    public class TooltipHoverEventArgs : EventArgs
    {
        public TooltipHoverEventArgs(
            ITooltipTargetProvider targetProvider,
            object[] tooltipContent,
            Bounds bounds
        )
        {
            TargetProvider = targetProvider;
            TooltipContent = tooltipContent;
            Bounds = bounds;
        }

        public ITooltipTargetProvider TargetProvider { get; }
        public object[] TooltipContent { get; }
        public Bounds Bounds { get; }
    }
}
