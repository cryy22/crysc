using System;

namespace Crysc.UI.Tooltips
{
    public class TooltipPublisher
    {
        public event EventHandler<TooltipHoverEventArgs> Hovered;
        public event EventHandler Clicked;

        private static TooltipPublisher _instance;
        public static TooltipPublisher I => _instance ??= new TooltipPublisher();

        public bool Enabled { get; set; } = true;

        public void RegisterHover(ITooltipTargetProvider targetProvider)
        {
            if (!Enabled) return;

            Hovered?.Invoke(
                sender: this,
                e: new TooltipHoverEventArgs(
                    targetProvider: targetProvider,
                    tooltipContent: targetProvider.GetTooltipContent(),
                    dimensions: targetProvider.GetSize()
                )
            );
        }

        public void RegisterClick()
        {
            if (!Enabled) return;

            Clicked?.Invoke(sender: this, e: EventArgs.Empty);
        }
    }
}
