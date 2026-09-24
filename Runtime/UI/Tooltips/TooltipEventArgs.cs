#region

using System;

#endregion

namespace Crysc.UI.Tooltips
{
    public class TooltipEventArgs : EventArgs
    {
        public ITooltipTargetProvider TargetProvider { get; }
        public object Content { get; }

        public TooltipEventArgs(
            ITooltipTargetProvider targetProvider,
            object content
        )
        {
            TargetProvider = targetProvider;
            Content = content;
        }
    }
}
