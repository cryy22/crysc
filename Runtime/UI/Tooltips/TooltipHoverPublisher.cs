using System;
using UnityEngine;

namespace Crysc.UI.Tooltips
{
    [CreateAssetMenu(fileName = "TooltipHoverPublisher", menuName = "Publishers/Tooltip Hover")]
    public class TooltipHoverPublisher : ScriptableObject
    {
        public event EventHandler<TooltipHoverEventArgs> Hovered;

        public void RegisterHover(ITooltipTargetProvider targetProvider)
        {
            Hovered?.Invoke(
                sender: this,
                e: new TooltipHoverEventArgs(
                    targetProvider: targetProvider,
                    tooltipContent: targetProvider.GetTooltipContent(),
                    bounds: targetProvider.GetBounds()
                )
            );
        }
    }
}
