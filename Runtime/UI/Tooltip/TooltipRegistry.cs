using System;
using Crysc.Registries;
using UnityEngine;

namespace Crysc.UI.Tooltip
{
    public abstract class TooltipRegistry<T> : Registry<T>
        where T : Component
    {
        public event EventHandler Hovered;
        public event EventHandler Unhovered;

        protected override void SubscribeToEvents(IRegistrar<T> registrar)
        {
            base.SubscribeToEvents(registrar);
            if (registrar is not ITooltipRegistrar<T> tRegistrar) return;

            tRegistrar.Hovered += HoveredEventHandler;
            tRegistrar.Unhovered += UnhoveredEventHandler;
        }

        protected override void UnsubscribeFromEvents(IRegistrar<T> registrar)
        {
            base.UnsubscribeFromEvents(registrar);
            if (registrar is not ITooltipRegistrar<T> tRegistrar) return;

            tRegistrar.Hovered -= HoveredEventHandler;
            tRegistrar.Unhovered -= UnhoveredEventHandler;
        }

        private void HoveredEventHandler(object sender, EventArgs e) { Hovered?.Invoke(sender: sender, e: e); }
        private void UnhoveredEventHandler(object sender, EventArgs e) { Unhovered?.Invoke(sender: sender, e: e); }
    }
}
