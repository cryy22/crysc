using System;
using UnityEngine;

namespace Crysc.UI.Tooltips
{
    [CreateAssetMenu(fileName = "TooltipHoverPublisher", menuName = "crysc/Publishers/Tooltip Hover")]
    public class TooltipHoverPublisher : ScriptableObject
    {
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

        public event EventHandler<TooltipHoverEventArgs> Hovered;
    }
}
