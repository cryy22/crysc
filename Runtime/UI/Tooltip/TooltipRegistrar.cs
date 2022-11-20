using System;
using Crysc.Registries;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crysc.UI.Tooltip
{
    [RequireComponent(typeof(Collider))]
    public abstract class TooltipRegistrar<T> : Registrar<T>, ITooltipRegistrar<T>,
        IPointerEnterHandler, IPointerExitHandler
        where T : Component
    {
        private void OnDisable() { Unhovered?.Invoke(sender: Registrant, e: EventArgs.Empty); }
        public void OnPointerEnter(PointerEventData _) { Hovered?.Invoke(sender: Registrant, e: EventArgs.Empty); }
        public void OnPointerExit(PointerEventData _) { Unhovered?.Invoke(sender: Registrant, e: EventArgs.Empty); }

        public event EventHandler Hovered;
        public event EventHandler Unhovered;
    }
}
