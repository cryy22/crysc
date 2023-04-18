using System;
using UnityEngine;

namespace Crysc.UI.Tooltips
{
    public class TooltipHoverEventArgs : EventArgs
    {
        public TooltipHoverEventArgs(
            ITooltipTargetProvider targetProvider,
            TooltipContent[] tooltipContent,
            Bounds bounds
        )
        {
            TargetProvider = targetProvider;
            TooltipContent = tooltipContent;
            Bounds = bounds;
        }

        public ITooltipTargetProvider TargetProvider { get; }
        public TooltipContent[] TooltipContent { get; }
        public Bounds Bounds { get; }
    }
}
