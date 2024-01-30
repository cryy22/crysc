using System;

namespace Crysc.UI.Tooltips
{
    public class TooltipPublisher
    {
        public event EventHandler<TooltipEventArgs> Hovered;
        public event EventHandler<TooltipEventArgs> Clicked;

        private static TooltipPublisher _instance;
        public static TooltipPublisher I => _instance ??= new TooltipPublisher();

        public bool Enabled { get; set; } = true;

        public void RegisterHover(ITooltipTargetProvider targetProvider)
        {
            if (!Enabled) return;

            Hovered?.Invoke(
                sender: this,
                e: new TooltipEventArgs(
                    targetProvider: targetProvider,
                    tooltipContent: targetProvider.GetTooltipContent(),
                    dimensions: targetProvider.GetSize()
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
                    tooltipContent: targetProvider.GetTooltipContent(),
                    dimensions: targetProvider.GetSize()
                )
            );
        }
    }
}
