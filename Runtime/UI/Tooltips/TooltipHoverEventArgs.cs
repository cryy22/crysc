using System;
using Crysc.Common;

namespace Crysc.UI.Tooltips
{
    public class TooltipHoverEventArgs : EventArgs
    {
        public ITooltipTargetProvider TargetProvider { get; }
        public object[] TooltipContent { get; }
        public Dimensions Dimensions { get; }

        public TooltipHoverEventArgs(
            ITooltipTargetProvider targetProvider,
            object[] tooltipContent,
            Dimensions dimensions
        )
        {
            TargetProvider = targetProvider;
            TooltipContent = tooltipContent;
            Dimensions = dimensions;
        }
    }
}
