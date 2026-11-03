#region

using System;
using Crysc.Patterns;

#endregion

namespace Crysc.UI.Tooltips
{
    public class TooltipPublisher : CSharpSingleton<TooltipPublisher>
    {
        public event Action<TooltipEventContext> Hovered;
        public event Action<TooltipEventContext> Unhovered;
        public event Action<TooltipEventContext> Clicked;

        public bool Enabled { get; set; } = true;

        public void RegisterHover(ITooltipTargetProvider targetProvider)
        {
            if (!Enabled)
                return;

            Hovered?.Invoke(
                new TooltipEventContext(
                    targetProvider: targetProvider,
                    content: targetProvider.GetTooltipContent()
                )
            );
        }

        public void RegisterUnhover(ITooltipTargetProvider targetProvider)
        {
            if (!Enabled)
                return;

            Unhovered?.Invoke(
                new TooltipEventContext(
                    targetProvider: targetProvider,
                    content: targetProvider.GetTooltipContent()
                )
            );
        }

        public void RegisterClick(ITooltipTargetProvider targetProvider)
        {
            if (!Enabled) return;

            Clicked?.Invoke(
                new TooltipEventContext(
                    targetProvider: targetProvider,
                    content: targetProvider.GetTooltipContent()
                )
            );
        }
    }
}
