#region

using System;
using Crysc.Patterns;

#endregion

namespace Crysc.UI.Tooltips
{
    public class TooltipPublisher : CSharpSingleton<TooltipPublisher>
    {
        public event EventHandler<TooltipEventArgs> Hovered;
        public event EventHandler<TooltipEventArgs> Unhovered;
        public event EventHandler<TooltipEventArgs> Clicked;

        public bool Enabled { get; set; } = true;

        public void RegisterHover(ITooltipTargetProvider targetProvider)
        {
            if (!Enabled)
                return;

            Hovered?.Invoke(
                sender: this,
                e: new TooltipEventArgs(
                    targetProvider: targetProvider,
                    tooltipContent: targetProvider.GetTooltipContent()
                )
            );
        }

        public void RegisterUnhover(ITooltipTargetProvider targetProvider)
        {
            if (!Enabled)
                return;

            Unhovered?.Invoke(
                sender: this,
                e: new TooltipEventArgs(
                    targetProvider: targetProvider,
                    tooltipContent: targetProvider.GetTooltipContent()
                )
            );
        }

        public void RegisterClick(ITooltipTargetProvider targetProvider)
        {
            if (!Enabled) return;

            Clicked?.Invoke(
                sender: this,
                e: new TooltipEventArgs(
                    targetProvider: targetProvider,
                    tooltipContent: targetProvider.GetTooltipContent()
                )
            );
        }
    }
}
