using System;
using Crysc.Common;

namespace Crysc.UI.Tooltips
{
    public class TooltipEventArgs : EventArgs
    {
        public ITooltipTargetProvider TargetProvider { get; }
        public object[] TooltipContent { get; }

        public TooltipEventArgs(
            ITooltipTargetProvider targetProvider,
            object[] tooltipContent
        )
        {
            TargetProvider = targetProvider;
            TooltipContent = tooltipContent;
        }
    }
}
