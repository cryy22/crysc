using System;

namespace Crysc.UI.Tooltips
{
    public class TooltipHoverPublisher
    {
        public event EventHandler<TooltipHoverEventArgs> Hovered;

        private static TooltipHoverPublisher _instance;
        public static TooltipHoverPublisher I => _instance ??= new TooltipHoverPublisher();

        public void RegisterHover(ITooltipTargetProvider targetProvider)
        {
            Hovered?.Invoke(
                sender: this,
                e: new TooltipHoverEventArgs(
                    targetProvider: targetProvider,
                    tooltipContent: targetProvider.GetTooltipContent(),
                    dimensions: targetProvider.GetSize()
                )
            );
        }
    }
}
